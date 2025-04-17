$(document).ready(function(){
    $('.slick-slider').slick({
      infinite: true,
      slidesToShow: 1,
      slidesToScroll: 1,
      dots: false,
      arrows: true,
      autoplay: true,
      autoplaySpeed: 2000,
      prevArrow: '<button type="button" class="slick-prev"></button>',
      nextArrow: '<button type="button" class="slick-next"></button>',
      
    });
  });

  const toggleBtn = document.querySelector('.mobile-toggle');
  const navMenu = document.querySelector('.header-nav__menu');

  toggleBtn.addEventListener('click', () => {
    navMenu.classList.toggle('show');
  });

