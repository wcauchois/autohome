var express = require('express'),
    config = require('config'),
    spawn = require('child_process').spawn;

var MAX_VOLUME = 65536;

var app = express();
app.use(express.bodyParser());
app.use(express.static(__dirname + '/static'));
app.use(express.cookieParser());

app.post('/volume/set', function(req, res) {
  var volumeInt = Math.round(req.body.volume * MAX_VOLUME);
  var proc = spawn('pactl', ['set-sink-volume', config.sink_device, volumeInt.toString()]);
  proc.on('exit', function(code) {
    res.send({ });
  });
});
app.get('/volume/get', function(req, res) {
  var proc = spawn('pacmd', ['list-sinks'], { stdio: ['ignore', 'pipe', 'ignore'] });
  proc.stdout.setEncoding('utf8');
  var output = '';
  proc.stdout.on('readable', function() {
    output += proc.stdout.read();
  });
  proc.stdout.on('end', function() {
    var lines = output.split('\n').map(function(s) { return s.trim() });
    var volume = null, useNextVolume = false;

    for (var i = 0; i < lines.length; i++) {
      var line = lines[i];
      if (/name:/.test(line) && line.indexOf(config.sink_device) >= 0) {
        useNextVolume = true;
      }
      if (/volume:/.test(line)) {
        // This is a line of the form "volume: (<channel>: <volume>%)*"; let's just
        // take the first channel's volume
        var parts = line.split(' ').filter(function(s) { return s.trim().length > 0; });
        volume = parseInt(parts[2].replace('%', '')) / 100.0;
        break;
      }
    }
    if (volume != null) {
      res.send({ volume: volume });
    } else res.send(500);
  });
});

app.listen(config.control_port);
console.log('Listening on port ' + config.control_port);

