// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.
//custom nav toggle on small screens
$(document).ready(function () {
  $('#navbarToggler').click(function () {
    $('.open-icon').toggle()
    $('.close-icon').toggle()
  })
})

$(document).ready(function () {
  const postNav = $('#id-post-nav')
  $('#navbarToggler').click(function () {
    var isExpanded = $(this).attr('aria-expanded') === 'true'
    if (isExpanded) {
      postNav.removeClass('post-navbar--fixed')
    }
  })
})

//navbar when scroll
$(document).ready(function () {
  const navbar = $('#nav-fixed-top')
  const postNav = $('#id-post-nav')

  let isScrollingUp = true
  let previousScrollPosition = $(window).scrollTop()

  $(window).scroll(function () {
    const currentScrollPosition = $(this).scrollTop()
    const scrollDirection =
      currentScrollPosition > 0 &&
      currentScrollPosition > previousScrollPosition
        ? 'down'
        : 'up'
    var navbarCollapse = $('.navbar-collapse')
    var navbarToggler = $('#navbarToggler')
    if (scrollDirection === 'up' && !isScrollingUp) {
      navbar.css('display', 'flex')
      postNav.css('top', '4.5rem')
      isScrollingUp = true
    } else if (scrollDirection === 'down' && isScrollingUp) {
      navbar.css('display', 'none')
      postNav.css('top', '0')
      postNav.addClass('post-navbar--fixed')
      if (navbarToggler.attr('aria-expanded') === 'true') {
        navbarCollapse.removeClass('show')
        navbarToggler.attr('aria-expanded', false)
        navbarToggler.removeClass('collapsed')
        $('.close-icon').css('display', 'none')
        $('.open-icon').css('display', 'inline')
      }

      isScrollingUp = false
    } else if (currentScrollPosition == 0) {
      // navbar.addClass('fixed-navbar')
      postNav.removeClass('post-navbar--fixed')
    }
    previousScrollPosition = currentScrollPosition
  })

  function isReloadedPage() {
    return (
      performance && performance.navigation && performance.navigation.type === 1
    )
  }
})

// Porfolio Area
// Add or remove 'active' class based on scroll position
