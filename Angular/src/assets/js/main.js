$(document).ready(function () {

    // :: 1.0 Preloader Active Code
    // browserWindow.on('load', function () {
    //     $('.preloader').fadeOut('slow', function () {
    //         $(this).remove();
    //     });
    // });

    //    hover-menu-overlay--------------------------
    $('li.nav-overlay').hover(function () {
        $('.mega-menu-level-two').removeClass('active');
        $('.nav-categories-overlay').addClass('active');
    }, function () {
        $('.nav-categories-overlay').removeClass('active');
    });

    //    resposive-megamenu-mobile------------------
    $('.dropdown-toggle').on('click', function (e) {
        e.stopPropagation();
        e.preventDefault();

        var self = $(this);
        if (self.is('.disabled, :disabled')) {
            return false;
        }
        self.parent().toggleClass("open");
    });

    $(document).on('click', function (e) {
        if ($('.dropdown').hasClass('open')) {
            $('.dropdown').removeClass('open');
        }
    });

    $('.nav-btn.nav-slider').on('click', function () {
        $('.overlay').show();
        $('nav').toggleClass("open");
    });

    $('.overlay').on('click', function () {
        if ($('nav').hasClass('open')) {
            $('nav').removeClass('open');
        }
        $(this).hide();
    });

    $('li.active').addClass('open').children('ul').show();
    $("li.has-sub > a").on('click', function () {
        $(this).removeAttr('href');
        var e = $(this).parent('li');
        if (e.hasClass('open')) {
            e.removeClass('open');
            e.find('li').removeClass('opne');
            e.find('ul').slideUp(200);
        }
        else {
            e.addClass('open');
            e.children('ul').slideDown(200);
            e.siblings('li').children('ul').slideUp(200);
            e.siblings('li').removeClass('open');
            e.siblings('li').find('li').removeClass('open');
            e.siblings('li').find('ul').slideUp(200);
        }
    });
    //    resposive-megamenu-mobile------------------

    // slider-product------------------------
    $(".product-carousel").owlCarousel({
        rtl: true,
        margin: 10,
        nav: true,
        navText: ['<i class="fa fa-angle-right"></i>', '<i class="fa fa-angle-left"></i>'],
        dots: false,
        responsiveClass: true,
        responsive: {
            0: {
                items: 1,
                slideBy: 1
            },
            576: {
                items: 1,
                slideBy: 1
            },
            768: {
                items: 3,
                slideBy: 2
            },
            992: {
                items: 4,
                slideBy: 2
            },
            1400: {
                items: 4,
                slideBy: 3
            }
        }
    });

    // brand---------------------------------------
    $(".product-carousel-brand").owlCarousel({
        items: 4,
        rtl: true,
        margin: 10,
        nav: true,
        navText: ['<i class="fa fa-angle-right"></i>', '<i class="fa fa-angle-left"></i>'],
        dots: false,
        responsiveClass: true,
        responsive: {
            0: {
                items: 1,
                slideBy: 1
            },
            576: {
                items: 1,
                slideBy: 1
            },
            768: {
                items: 3,
                slideBy: 2
            },
            992: {
                items: 5,
                slideBy: 2
            },
            1400: {
                items: 5,
                slideBy: 3
            }
        }
    });
    // brand---------------------------------------

    // Symbol--------------------------------------
    $(".product-carousel-symbol").owlCarousel({
        rtl: true,
        items: 2,
        loop: true,
        margin: 10,
        dots: false,
        autoplay: true,
        autoplayTimeout: 3000,
        autoplayHoverPause: true,
        responsiveClass: true,
        responsive: {
            0: {
                items: 1,
                slideBy: 1,
                autoplay: true,
            },
            576: {
                items: 1,
                slideBy: 1,
                autoplay: true,
            },
            768: {
                items: 1,
                slideBy: 1,
                autoplay: true,
            },
            992: {
                items: 1,
                slideBy: 1,
                autoplay: true,
            },
            1400: {
                items: 1,
                slideBy: 1,
                autoplay: true,
            }
        }
    });
    // Symbol--------------------------------------

    $("#suggestion-slider").owlCarousel({
        rtl: true,
        items: 1,
        autoplay: true,
        autoplayTimeout: 5000,
        loop: true,
        dots: false,
        onInitialized: startProgressBar,
        onTranslate: resetProgressBar,
        onTranslated: startProgressBar
    });

    function startProgressBar() {
        $(".slide-progress").css({
            width: "100%",
            transition: "width 5000ms"
        });
    }

    function resetProgressBar() {
        $(".slide-progress").css({
            width: 0,
            transition: "width 0s"
        });
    }

    // sidebar-sticky-------------------------
    if ($('.sticky-sidebar').length) {
        $('.sticky-sidebar').theiaStickySidebar();
    }

    //   countdown----------------------------
    ! function (l) {
        var t = {
            init: function () {
                t.countDown()
            },
            countDown: function (t, i) {
                l(".countdown").each(function () {
                    var t = l(this),
                        a = l(this).data("date-time"),
                        e = l(this).data("labels");
                    (i || t).countdown(a, function (t) {
                        l(this).html(t.strftime('<div class="countdown-item"><div class="countdown-value">%D</div><div class="countdown-label">' + e["label-day"] + '</div></div><div class="countdown-item"><div class="countdown-value">%H</div><div class="countdown-label">' + e["label-hour"] + '</div></div><div class="countdown-item"><div class="countdown-value">%M</div><div class="countdown-label">' + e["label-minute"] + '</div></div><div class="countdown-item"><div class="countdown-value">%S</div><div class="countdown-label">' + e["label-second"] + "</div></div>"))
                    })
                })
            },
        };
        l(function () {
            t.init()
        })
    }(jQuery);
    const cd = new Date().getFullYear() + 1
    $('#countdown').countdown({
        year: cd
    });

    // checkout-coupon-------------------------------
    $(".showcoupon").on("click", function () {
        $(".checkout-coupon").slideToggle(200);
    });
    // checkout-coupon-------------------------------

    // add-product-wishes----------------------------
    $("ul.gallery-actions li .add-product-wishes").on("click", function () {
        $(this).toggleClass("active");
    });
    // add-product-wishes----------------------------

    // nice-select-----------------------------------
    if ($('.custom-select-ui').length) {
        $('.custom-select-ui select').niceSelect();
    }

    //    price-range--------------------------------
    var nonLinearStepSlider = document.getElementById('slider-non-linear-step');

    if ($('#slider-non-linear-step').length) {
        noUiSlider.create(nonLinearStepSlider, {
            start: [0, 5000000],
            connect: true,
            direction: 'rtl',
            format: wNumb({
                decimals: 0,
                thousand: ','
            }),
            range: {
                'min': [0],
                '10%': [500, 500],
                '50%': [40000, 1000],
                'max': [10000000]
            }
        });
        var nonLinearStepSliderValueElement = document.getElementById('slider-non-linear-step-value');

        nonLinearStepSlider.noUiSlider.on('update', function (values) {
            nonLinearStepSliderValueElement.innerHTML = values.join(' - ');
        });
    }
    //    price-range--------------------------

    //    quantity-selector--------------------
    jQuery('<div class="quantity-nav"><div class="quantity-button quantity-up">+</div><div class="quantity-button quantity-down">-</div></div>').insertAfter('.quantity input');
    jQuery('.quantity').each(function () {
        var spinner = jQuery(this),
            input = spinner.find('input[type="number"]'),
            btnUp = spinner.find('.quantity-up'),
            btnDown = spinner.find('.quantity-down'),
            min = input.attr('min'),
            max = input.attr('max');

        btnUp.click(function () {
            var oldValue = parseFloat(input.val());
            if (oldValue >= max) {
                var newVal = oldValue;
            } else {
                var newVal = oldValue + 1;
            }
            spinner.find("input").val(newVal);
            spinner.find("input").trigger("change");
        });

        btnDown.click(function () {
            var oldValue = parseFloat(input.val());
            if (oldValue <= min) {
                var newVal = oldValue;
            } else {
                var newVal = oldValue - 1;
            }
            spinner.find("input").val(newVal);
            spinner.find("input").trigger("change");
        });

    });
    //    quantity-selector---------------------------

    // scroll_progress-------------------------   
    var progressPath = document.querySelector('.progress-wrap path');
    if (progressPath != null) {
        var pathLength = progressPath.getTotalLength();
        progressPath.style.transition = progressPath.style.WebkitTransition = 'none';
        progressPath.style.strokeDasharray = pathLength + ' ' + pathLength;
        progressPath.style.strokeDashoffset = pathLength;
        progressPath.getBoundingClientRect();
        progressPath.style.transition = progressPath.style.WebkitTransition = 'stroke-dashoffset 10ms linear';
        var updateProgress = function () {
            var scroll = $(window).scrollTop();
            var height = $(document).height() - $(window).height();
            var progress = pathLength - (scroll * pathLength / height);
            progressPath.style.strokeDashoffset = progress;
        }
    }
    updateProgress();
    $(window).scroll(updateProgress);
    var offset = 50;
    var duration = 1500;
    jQuery(window).on('scroll', function () {
        if (jQuery(this).scrollTop() > offset) {
            jQuery('.progress-wrap').addClass('active-progress');
        } else {
            jQuery('.progress-wrap').removeClass('active-progress');
        }
    });
    jQuery('.progress-wrap').on('click', function (event) {
        event.preventDefault();
        jQuery('html, body').animate({ scrollTop: 0 }, duration);
        return false;
    });

    //    verify-phone-number------------------------
    if ($("#countdown-verify-end").length) {
        var $countdownOptionEnd = $("#countdown-verify-end");

        $countdownOptionEnd.countdown({
            date: (new Date()).getTime() + 180 * 1000, // 1 minute later
            text: '<span class="day">%s</span><span class="hour">%s</span><span>: %s</span><span>%s</span>',
            end: function () {
                $countdownOptionEnd.html("<a href='' class='link-border-verify form-account-link'>ارسال مجدد</a>");
            }
        });
    }
    $(".line-number-account").keyup(function () {
        $(this).next().focus();
    });
    //    verify-phone-number-----------------------

    // tab-------------------------------------
    $(".mask-handler").click(function (e) {
        e.preventDefault();
        var sumaryBox = $(this).parents('.content-expert-summary');
        sumaryBox.find('.mask-text').toggleClass('active');
        sumaryBox.find('.shadow-box').fadeToggle(0);
        $(this).find('.show-more').fadeToggle(0);
        $(this).find('.show-less').fadeToggle(0);
    });

    $(".content-expert-button").click(function (e) {
        e.preventDefault();
        var sumaryBox = $(this).parents('.content-expert-article');
        sumaryBox.find('.content-expert-article').toggleClass('active');
        sumaryBox.find('.content-expert-text').slideToggle();
        $(this).find('.show-more').fadeToggle(0);
        $(this).find('.show-less').fadeToggle(0);
    });
    // tab-------------------------------------

    // product-img-----------------------------
    $("#gallery-slider").owlCarousel({
        rtl: true,
        margin: 10,
        nav: true,
        navText: ['<i class="fa fa-angle-right"></i>', '<i class="fa fa-angle-left"></i>'],
        dots: false,
        responsiveClass: true,
        responsive: {
            0: {
                items: 4,
                slideBy: 1
            }
        }
    });

    $('.back-to-top').click(function (e) {
        e.preventDefault();
        $('html, body').animate({ scrollTop: 0 }, 800, 'easeInExpo');
    });

    if ($("#img-product-zoom").length) {
        $("#img-product-zoom").ezPlus({
            zoomType: "inner",
            containLensZoom: true,
            gallery: 'gallery_01f',
            cursor: "crosshair",
            galleryActiveClass: "active",
            responsive: true,
            imageCrossfade: true,
            zoomWindowFadeIn: 500,
            zoomWindowFadeOut: 500
        });
    }

    // var $customEvents = $('#custom-events');
    // $customEvents.lightGallery();

    // var colours = ['#21171A', '#81575E', '#9C5043', '#8F655D'];
    // $customEvents.on('onBeforeSlide.lg', function(event, prevIndex, index){
    //     $('.lg-outer').css('background-color', colours[index])
    // });
    // product-img-----------------------------

});

//////////////////////// cartIndex ////////////////////////
var current_fs, next_fs, previous_fs; //fieldsets
var left, opacity, scale; //fieldset properties which we will animate
var animating; //flag to prevent quick multi-click glitches

$(document).on('click', '.next', function () {
    if (animating) return false;
    animating = true;

    current_fs = $(this).parent();
    next_fs = $(this).parent().next();

    //activate next step on progressbar using the index of next_fs
    $("#progressbar li").eq($("fieldset").index(next_fs)).addClass("active");

    //show the next fieldset
    next_fs.show();
    //hide the current fieldset with style
    current_fs.animate({ opacity: 0 }, {
        step: function (now, mx) {
            //as the opacity of current_fs reduces to 0 - stored in "now"
            //1. scale current_fs down to 80%
            scale = 1 - (1 - now) * 0.2;
            //2. bring next_fs from the right(50%)
            left = (now * 50) + "%";
            //3. increase opacity of next_fs to 1 as it moves in
            opacity = 1 - now;
            current_fs.css({ 'transform': 'scale(' + scale + ')' });
            next_fs.css({ 'left': left, 'opacity': opacity });
        },
        duration: 800,
        complete: function () {
            current_fs.hide();
            animating = false;
        },
        //this comes from the custom easing plugin
        easing: 'easeInOutBack'
    });
});
$(document).on('click', '.previous', function () {
    if (animating) return false;
    animating = true;

    current_fs = $(this).parent();
    previous_fs = $(this).parent().prev();

    //de-activate current step on progressbar
    $("#progressbar li").eq($("fieldset").index(current_fs)).removeClass("active");

    //show the previous fieldset
    previous_fs.show();
    //hide the current fieldset with style
    current_fs.animate({ opacity: 0 }, {
        step: function (now, mx) {
            //as the opacity of current_fs reduces to 0 - stored in "now"
            //1. scale previous_fs from 80% to 100%
            scale = 0.8 + (1 - now) * 0.2;
            //2. take current_fs to the right(50%) - from 0%
            left = ((1 - now) * 50) + "%";
            //3. increase opacity of previous_fs to 1 as it moves in
            opacity = 1 - now;
            current_fs.css({ 'left': left });
            previous_fs.css({ 'transform': 'scale(' + scale + ')', 'opacity': opacity });
        },
        duration: 800,
        complete: function () {
            current_fs.hide();
            animating = false;
        },
        //this comes from the custom easing plugin
        easing: 'easeInOutBack'
    });
});

//////////////////////// show categories menu ////////////////////////
function toggle() {
    document.getElementById("collapse").classList.toggle("show");
}

//////////////////////// close categories collapse ////////////////////////
$(document).on('click', '.current-link-menu', function (e) {
    const collapse = document.getElementById('collapse');
    collapse.style.display = 'flex';
    $(window).click(function (e) {
        if (collapse.previousSibling != e.target)
            collapse.style.display = 'none';
    });
})

//////////////////////// show productInCategory ////////////////////////
window.onload = function () {
    const btn = document.getElementById('pill-active');
    if (btn != null && btn != undefined)
        btn.click();
}

//////////////////////// Animated Header On Scroll ////////////////////////
$(window).scroll(function () {
    var header = $(document).scrollTop();
    var headerHeight = $("#secondaryMenu").outerHeight();
    var firstSection = $(".container-main  section:nth-of-type(1)").outerHeight();
    if (header > headerHeight * 3) {
        $("#secondaryMenu").addClass("fixed");
        $("#secondary-search").addClass("fixed");
        $("#close-menu").addClass("fixed");
    }
    else {
        $("#secondaryMenu").removeClass("fixed");
        $("#secondary-search").removeClass("fixed");
        $("#close-menu").removeClass("fixed");
        document.getElementById('secondaryMenu').style.display = 'inline-block';
    }
    if (header > firstSection * 3) {
        $("#secondaryMenu").addClass("in-view");
        $("#secondary-search").addClass("in-view");
        $("#close-menu").addClass("in-view");
    }
    else {
        $("#secondaryMenu").removeClass("in-view");
        $("#secondary-search").removeClass("in-view");
        $("#close-menu").removeClass("in-view");
    }

    $(document).on('click', '.close-header', function () {
        document.getElementById('secondaryMenu').style.display = 'none';
    });
});

//////////////////////// product detail ////////////////////////
document.addEventListener("DOMContentLoaded", function (e) {
    const activeImage = document.querySelector(".product-image .active");
    $(activeImage).on("load", function (e) {
        const productImages = document.querySelectorAll(".image-list img");

        function changeImage(e) {
            activeImage.src = e.target.src;
        }

        productImages.forEach(image => image.addEventListener("click", changeImage));
    });
});

window.addEventListener('DOMContentLoaded', () => {
    const step = document.querySelector('.step');
    if (step != null && step != undefined) {
        $(step).load(function (e) {
            const ready = document.getElementById('#ready');
            const confirm = document.getElementById('#confirm');
            const proccess = document.getElementById('#proccess');
            const delivery = document.getElementById('#delivery');
            if (proccess.hasClass != 'acrive') {
                proccess.setAttribute("class", 'active');
            }
            else if (confirm.hasClass != 'acrive') {
                confirm.setAttribute("class", 'active');
            }
            else if (delivery.hasClass != 'acrive') {
                delivery.setAttribute("class", 'active');
            }
            else if (ready.hasClass != 'acrive') {
                ready.setAttribute("class", 'active');
            }
        });
    }
});

window.addEventListener('DOMContentLoaded', () => {
    // Crop image
    var $uploadCrop,
        tempFilename,
        rawImg,
        imageId;

    function readFile(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('.upload-demo').addClass('ready');
                $('#cropImagePop').modal('show');
                rawImg = e.target.result;
            }
            reader.readAsDataURL(input.files[0]);
        }
        else {
            swal("Sorry - you're browser doesn't support the FileReader API");
        }
    }

    $uploadCrop = $('#upload-demo').croppie({
        viewport: {
            width: 740,
            height: 500,
        },
        enforceBoundary: false,
        enableExif: true
    });

    $('#cropImagePop').on('shown.bs.modal', function () {
        $uploadCrop.croppie('bind', {
            url: rawImg
        }).then(function () {
            console.log('jQuery bind complete');
        });
    });

    $('.item-img').on('change', function () {
        imageId = $(this).data('id'); tempFilename = $(this).val();
        $('#cancelCropBtn').data('id', imageId); readFile(this);
    });

    $('#cropImageBtn').on('click', function (ev) {
        $uploadCrop.croppie('result', {
            type: 'base64',
            format: 'jpeg',
            size: { width: 740, height: 500 }
        }).then(function (resp) {
            $('#item-img-output').attr('src', resp);
            $("#base64").val(resp.split(',')[1]);
            $('#cropImagePop').modal('hide');
        });
    });
});
