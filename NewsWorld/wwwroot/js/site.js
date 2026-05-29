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






