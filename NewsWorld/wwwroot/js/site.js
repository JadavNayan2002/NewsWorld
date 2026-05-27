// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

@if (TempData["loginSuccess"] != null) {
    <script>

        Swal.fire({
            toast: true,
        position: 'top-end',
        icon: 'success',
        title: '@TempData["loginSuccess"]',
        showConfirmButton: false,
        timer: 3000
                    });

    </script>
}

@if (TempData["newsSuccess"] != null) {
    <script>

        Swal.fire({
            toast: true,
        position: 'top-end',
        icon: 'success',
        title: '@TempData["newsSuccess"]',
        showConfirmButton: false,
        timer: 3000
                    });

    </script>
}

@if (TempData["adminSuccess"] != null) {
    <script>

        Swal.fire({
            toast: true,
        position: 'top-end',
        icon: 'success',
        title: '@TempData["adminSuccess"]',
        showConfirmButton: false,
        timer: 3000
                    });

    </script>
}

// Sidebar active menu

const menuItems = document.querySelectorAll(".sidebar-menu a");

menuItems.forEach(item => {

    item.addEventListener("click", function () {

        menuItems.forEach(link => {
            link.classList.remove("active");
        });

        this.classList.add("active");

    });

});

// CK Editor





