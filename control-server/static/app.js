function updateVolumeDisplay(volume) {
  $('.volume-chiclet').removeClass('active');
  for (var i = 0; i < 5; i++) {
    var $chiclet = $('.level-' + (i + 1));
    var required = (i * 2) / 10.0;
    if (volume > required) $chiclet.addClass('active');
    $chiclet.css('opacity', (volume > required + (1 / 10.0)) ? '1' : '0.5');
  }
}
function refreshVolume() {
  $.get('/volume/get', function(data) {
    updateVolumeDisplay(data.volume);
  });
}

$(function() {
  $('#increase-volume').click(function() {
    $.post('/volume/increase', { }, function(data) {
      updateVolumeDisplay(data.volume);
    });
  });
  $('#decrease-volume').click(function() {
    $.post('/volume/decrease', { }, function(data) {
      updateVolumeDisplay(data.volume);
    });
  });
  $('#mute-volume').click(function() {
    $.post('/volume/mute', { }, function(data) {
      updateVolumeDisplay(data.volume);
    });
  });
  refreshVolume();
  setInterval(refreshVolume, 500);
});
