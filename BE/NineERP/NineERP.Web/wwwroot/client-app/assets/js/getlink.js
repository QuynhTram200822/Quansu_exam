$('.news-item').click(function () {
  if ($(this).find('a').attr('target') == '_blank') {
    window.open($(this).find('a').attr('href'), '_blank');
  } else {
    window.location = $(this).find('a').attr('href');
  }
  return false;
});

$('.notify-item').click(function () {
  if ($(this).find('a').attr('target') == '_blank') {
    window.open($(this).find('a').attr('href'), '_blank');
  } else {
    window.location = $(this).find('a').attr('href');
  }
  return false;
});
$('.news-card__item').click(function () {
  if ($(this).find('a').attr('target') == '_blank') {
    window.open($(this).find('a').attr('href'), '_blank');
  } else {
    window.location = $(this).find('a').attr('href');
  }
  return false;
});

