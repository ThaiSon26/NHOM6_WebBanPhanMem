/**
 * SoftDev Admin - Core Interactions Script
 * White + Blue Clean SaaS Developer Marketplace
 */
document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.getElementById("adminSidebar");
    const toggleBtn = document.getElementById("sidebarToggleBtn");
    const topbarToggleBtn = document.getElementById("topbarSidebarToggleBtn");
    const brandPill = document.querySelector(".brand-pill");
    const searchBtn = document.getElementById("topbarSearchBtn");

    function updateIcons(isCollapsed) {
        // Cập nhật nút trong thanh Sidebar
        if (toggleBtn) {
            const icon = toggleBtn.querySelector("i");
            if (icon) {
                icon.className = isCollapsed ? "bi bi-chevron-right" : "bi bi-chevron-left";
            }
            toggleBtn.title = isCollapsed ? "Mở rộng thanh menu" : "Thu gọn thanh menu";
        }

        // Cập nhật nút trên thanh Topbar
        if (topbarToggleBtn) {
            const topbarIcon = topbarToggleBtn.querySelector("i");
            if (topbarIcon) {
                topbarIcon.className = isCollapsed ? "bi bi-layout-sidebar" : "bi bi-layout-sidebar-inset";
            }
            topbarToggleBtn.title = isCollapsed ? "Mở rộng thanh menu" : "Thu gọn thanh menu";
        }

        // Cập nhật tooltip cho logo brand khi thu gọn
        if (brandPill) {
            brandPill.title = isCollapsed ? "Bấm vào để mở rộng menu" : "Trang quản trị SoftDev";
        }
    }

    function toggleMenu() {
        if (!sidebar) return;
        sidebar.classList.toggle("collapsed");
        const isCollapsed = sidebar.classList.contains("collapsed");
        localStorage.setItem("softdev_sidebar_collapsed", isCollapsed ? "true" : "false");
        updateIcons(isCollapsed);
    }

    // Gắn sự kiện click cho nút thu/mở ở sidebar
    if (toggleBtn) {
        toggleBtn.addEventListener("click", function (e) {
            e.stopPropagation();
            toggleMenu();
        });
    }

    // Gắn sự kiện click cho nút thu/mở ở Topbar
    if (topbarToggleBtn) {
        topbarToggleBtn.addEventListener("click", function (e) {
            e.stopPropagation();
            toggleMenu();
        });
    }

    // Khi Sidebar đang thu gọn, bấm vào logo cũng bung mở ra
    if (brandPill) {
        brandPill.addEventListener("click", function (e) {
            if (sidebar && sidebar.classList.contains("collapsed")) {
                e.preventDefault();
                toggleMenu();
            }
        });
    }

    // Khởi tạo trạng thái thu gọn từ localStorage
    const isCollapsedSaved = localStorage.getItem("softdev_sidebar_collapsed") === "true";
    if (sidebar) {
        if (isCollapsedSaved) {
            sidebar.classList.add("collapsed");
        } else {
            sidebar.classList.remove("collapsed");
        }
        updateIcons(isCollapsedSaved);
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

    // Phím tắt bàn phím: Ctrl + B hoặc Ctrl + \ để mở/thu gọn Sidebar
    document.addEventListener("keydown", function (e) {
        if ((e.ctrlKey || e.metaKey) && (e.key.toLowerCase() === "b" || e.key === "\\")) {
            e.preventDefault();
            toggleMenu();
        }
    });
});
