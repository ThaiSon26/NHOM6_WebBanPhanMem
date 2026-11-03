/**
 * SoftDev Admin - Core Interactions Script
 * White + Blue Clean SaaS Developer Marketplace
 */
document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.getElementById("adminSidebar");
    const toggleBtn = document.getElementById("sidebarToggleBtn");
    const searchBtn = document.getElementById("topbarSearchBtn");

    function toggleMenu() {
        if (!sidebar) return;
        sidebar.classList.toggle("collapsed");
        const isCollapsed = sidebar.classList.contains("collapsed");
        localStorage.setItem("softdev_sidebar_collapsed", isCollapsed ? "true" : "false");

        // Đổi hướng icon chevron
        if (toggleBtn) {
            const icon = toggleBtn.querySelector("i");
            if (icon) {
                icon.className = isCollapsed ? "bi bi-chevron-right" : "bi bi-chevron-left";
            }
        }
    }

    if (toggleBtn) {
        toggleBtn.addEventListener("click", toggleMenu);
    }

    // Nhớ trạng thái thu gọn qua các phiên duyệt trang
    if (localStorage.getItem("softdev_sidebar_collapsed") === "true" && sidebar) {
        sidebar.classList.add("collapsed");
        if (toggleBtn) {
            const icon = toggleBtn.querySelector("i");
            if (icon) icon.className = "bi bi-chevron-right";
        }
    }

    // Nút tìm kiếm trên Topbar
    if (searchBtn) {
        searchBtn.addEventListener("click", function () {
            const catSearchInput = document.getElementById("categorySearchInput");
            if (catSearchInput) {
                catSearchInput.focus();
                catSearchInput.scrollIntoView({ behavior: 'smooth', block: 'center' });
            } else {
                window.location.href = "/Admin/DanhMuc";
            }
        });
    }

    // Phím tắt bàn phím tiện lợi: Ctrl + B để mở/thu gọn Sidebar
    document.addEventListener("keydown", function (e) {
        if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === "b") {
            e.preventDefault();
            toggleMenu();
        }
    });
});
