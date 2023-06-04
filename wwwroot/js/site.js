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
      navbar.removeClass('slide-up')
      isScrollingUp = true
    } else if (scrollDirection === 'down' && isScrollingUp) {
      // navbar.css('display', 'none')
      navbar.addClass('slide-up')
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

var isUserClickedButton = false
var pageNumber = 2 // Initial page number
var pageSize = 6 // Number of posts to load per page
var hasMorePosts = true
var btnMore = document.querySelector('.load-more-btn')

function loadMorePosts(controller) {
  if (isUserClickedButton && hasMorePosts) {
    btnMore.value = 'Loading more posts'
    $.ajax({
      url: `/${controller}/GetMorePosts`,
      type: 'GET',
      data: { pageNumber: pageNumber, pageSize: pageSize },
      success: function (data) {
        $('.post-container').append(data)
        pageNumber++ // Increment the page number for the next load more request
        btnMore.value = 'Load more posts' // Hide the "Load More" button if there are no more posts
      },
      error: function (xhr, textStatus, errorThrown) {
        if (xhr.status === 404) {
          // Handle Not Found response
          console.log('Resource not found.')
          hasMorePosts = false
          btnMore.value = 'No more posts'
        } else {
          // Handle other error responses
          console.log('An error occurred: ' + errorThrown)
        }
      },
    })
  } else {
    btnMore.value = 'No more posts'
  }
}
