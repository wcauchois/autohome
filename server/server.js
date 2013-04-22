var express = require('express'),
    config = require('config'),
    spawn = require('child_process').spawn;

var MAX_VOLUME = 65536;
var EPSILON = 0.001;

var app = express();
app.use(express.bodyParser());
app.use(express.static(__dirname + '/static'));
app.use(express.cookieParser());

var currentVolume = config.starting_volume;
var deviceSource = null, mediaSource = null, activeSource = null;
var sink = null;

app.use(function(req, res, next) {
  res.sendVolume = (function() {
    this.send({ volume: currentVolume });
  }).bind(res);
  next();
});

function closeEnough(value, target) {
  return (value + EPSILON > target && value - EPSILON < target);
}
function spawnNewSink() {
  return spawn('pacat', ['-d', config.sink_device,
    '--volume=' + Math.round(MAX_VOLUME * currentVolume),
    '--latency-msec=' + config.latency_msec],
    { stdio: ['pipe', 'ignore', 'ignore'] });
}
function restartSink(callback) {
  console.log('Restarting sink...');
  activeSource.stdout.unpipe();
  sink.on('exit', function() {
    sink = spawnNewSink();
    activeSource.stdout.pipe(sink.stdin);
    console.log('Restarted sink');
    callback();
  });
  sink.kill();
}
function switchSource(newSource) {
  var oldSource = activeSource;
  activeSource = newSource;
  oldSource.stdout.unpipe();
  newSource.stdout.pipe(sink.stdin);
  console.log('Switched source');
}
function updateMediaGain() {
  // mediaSource.stdin.write('GAIN ' + Math.round(currentVolume * 100) + '\n', 'utf8');
}
function setVolume(newVolume, callback) {
  if (newVolume > 1.0) newVolume = 1.0;
  if (newVolume < 0.0) newVolume = 0.0;
  console.log('Setting new volume: ' + newVolume);
  currentVolume = newVolume;
  updateMediaGain();
  restartSink(callback);
}

app.post('/volume/increase', function(req, res) {
  setVolume(currentVolume + config.volume_increment, res.sendVolume);
});
app.post('/volume/decrease', function(req, res) {
  setVolume(currentVolume - config.volume_increment, res.sendVolume);
});
app.post('/volume/mute', function(req, res) {
  if (closeEnough(currentVolume, 0.0)) {
    var targetVolume = 1.0;
    if (req.cookies.saved_volume) {
      targetVolume = parseFloat(req.cookies.saved_volume);
    }
    setVolume(targetVolume, res.sendVolume);
  } else {
    res.cookie('saved_volume', currentVolume.toString());
    setVolume(0.0, res.sendVolume);
  }
});
app.get('/volume/get', function(req, res) {
  res.send({ volume: currentVolume });
});

// XXX mpg321 seems to take up 100% cpu? maybe fixed on the bleeding edge tho
// mediaSource = spawn('mpg321', ['-R', 'dummy'], { stdio: ['pipe', 'pipe', 'ignore'] });
deviceSource = spawn('parec',
  ['-d', config.source_device, '--latency-msec=' + config.latency_msec],
  { stdio: ['ignore', 'pipe', 'ignore'] });
sink = spawnNewSink();
activeSource = deviceSource;
activeSource.stdout.pipe(sink.stdin);

app.listen(config.web_port);
console.log('Listening on port ' + config.web_port);

