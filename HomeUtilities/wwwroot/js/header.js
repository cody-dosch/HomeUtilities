$(document).ready(function () {
    // Toggle mobile accordions
    $('.navbar-nav .has-submenu > a').on('click', function (e) {
        if (window.innerWidth < 768) {
            e.preventDefault();
            $(this).parent('.has-submenu').toggleClass('show');
            $(this).next('.submenu').toggleClass('show');
        }
    });

    // Ensure only one mobile accordion is open at a time
    $('.navbar-nav .has-submenu > a').on('show.bs.collapse', function () {
        if (window.innerWidth < 768) {
            $('.navbar-nav .has-submenu .submenu').not($(this).next('.submenu')).removeClass('show');
            $('.navbar-nav .has-submenu').not($(this).parent('.has-submenu')).removeClass('show');
        }
    });

    // Add Bootstrap collapse functionality for mobile
    $('.navbar-nav .has-submenu > a').attr('data-bs-toggle', 'collapse');

    // Initialize Bootstrap collapse for mobile
    var collapseElementList = [].slice.call(document.querySelectorAll('.collapse'))
    var collapseList = collapseElementList.map(function (collapseEl) {
        return new bootstrap.Collapse(collapseEl, { toggle: false }) // Prevent initial toggle
    })
});