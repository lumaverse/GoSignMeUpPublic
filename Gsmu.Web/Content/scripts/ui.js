$(window).scroll(function () {
  RefreshNav()
});
$(window).resize(function() {
  RefreshNav()
  //COURSESEARCH.Invoke();
});
$(document).ready(function() {
  RefreshNav();
  $('ul.subcat').hide();
  $('ul.subsubcat').hide();
});

function RefreshNav(){
  if ($(window).scrollTop() >= 50) {
    $('.navbar').removeClass('transparent');
  } else {
    //$('.navbar').addClass('transparent');
    if ($(window).width() <= 768) {
      $('.navbar').removeClass('transparent');
    } else {
      $('.navbar').addClass('transparent');
    }
  }
}

// $('li.maincat').unbind('click').click(function() {
    // console.log('click maincat ')
    // $(this).children('ul.subcat').slideToggle();
// });
// $('li.subcat').unbind('click').click(function() {
  // console.log('click subcat ')
    // $(this).children('ul.subsubcat').slideToggle();
// });
