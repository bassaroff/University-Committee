
/* sticky Header */
$(window).on('scroll', function () {
    var scroll = $(window).scrollTop(),
        hbHeight = $('.header-top').innerHeight(),
        headerBottom = $('.header-bottom');

    if (scroll >= hbHeight) {
        headerBottom.addClass("sticky-header");
    } else {
        headerBottom.removeClass("sticky-header");
    }
});
$(window).on('scroll', function () {
    var scroll = $(window).scrollTop(),
        hbHeight = $('.header-top').innerHeight(),
        mobileMenu = $('.mobile-menu');

    if (scroll >= hbHeight) {
        mobileMenu.addClass("sticky-header");
    } else {
        mobileMenu.removeClass("sticky-header");
    }
});
