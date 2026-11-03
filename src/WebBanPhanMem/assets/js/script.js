/**
 * SoftDev - Global Script & Auth Interactions
 * Clean SaaS White + Blue
 */
document.addEventListener("DOMContentLoaded", function () {
    // ==========================================
    // 1. CHUYỂN ĐỔI TAB ĐĂNG NHẬP / ĐĂNG KÝ
    // ==========================================
    const tabLoginBtn = document.getElementById("tabLoginBtn");
    const tabRegisterBtn = document.getElementById("tabRegisterBtn");
    const formLogin = document.getElementById("formLogin");
    const formRegister = document.getElementById("formRegister");
    const authMainTitle = document.getElementById("authMainTitle");

    function switchAuthTab(tabName) {
        if (!tabLoginBtn || !tabRegisterBtn || !formLogin || !formRegister) return;

        if (tabName === "register") {
            tabLoginBtn.classList.remove("active");
            tabRegisterBtn.classList.add("active");
            formLogin.style.display = "none";
            formRegister.style.display = "block";
            if (authMainTitle) authMainTitle.textContent = "Tạo tài khoản mới";
        } else {
            tabRegisterBtn.classList.remove("active");
            tabLoginBtn.classList.add("active");
            formRegister.style.display = "none";
            formLogin.style.display = "block";
            if (authMainTitle) authMainTitle.textContent = "Đăng nhập tài khoản";
        }
    }

    if (tabLoginBtn) {
        tabLoginBtn.addEventListener("click", function () {
            switchAuthTab("login");
        });
    }

    if (tabRegisterBtn) {
        tabRegisterBtn.addEventListener("click", function () {
            switchAuthTab("register");
        });
    }

    // ==========================================
    // 2. ẨN / HIỆN MẬT KHẨU (EYE ICON TOGGLE)
    // ==========================================
    const eyeButtons = document.querySelectorAll(".auth-btn-eye");
    eyeButtons.forEach(btn => {
        btn.addEventListener("click", function () {
            const targetId = this.getAttribute("data-target");
            const input = document.getElementById(targetId);
            const icon = this.querySelector("i");

            if (input) {
                if (input.type === "password") {
                    input.type = "text";
                    if (icon) {
                        icon.className = "bi bi-eye-slash";
                    }
                } else {
                    input.type = "password";
                    if (icon) {
                        icon.className = "bi bi-eye";
                    }
                }
            }
        });
    });

    // ==========================================
    // 3. THANH ĐIỀU KHIỂN QUẢN TRỊ (ADMIN SIDEBAR TOGGLE)
    // ==========================================
    const sidebar = document.getElementById("adminSidebar");
    const toggleBtn = document.getElementById("sidebarToggleBtn");
    const topbarToggleBtn = document.getElementById("topbarSidebarToggleBtn");
    const brandPill = document.querySelector(".brand-pill");

    function updateSidebarIcons(isCollapsed) {
        if (toggleBtn) {
            const icon = toggleBtn.querySelector("i");
            if (icon) {
                icon.className = isCollapsed ? "bi bi-chevron-right" : "bi bi-chevron-left";
            }
            toggleBtn.title = isCollapsed ? "Mở rộng thanh menu" : "Thu gọn thanh menu";
        }

        if (topbarToggleBtn) {
            const topbarIcon = topbarToggleBtn.querySelector("i");
            if (topbarIcon) {
                topbarIcon.className = isCollapsed ? "bi bi-layout-sidebar" : "bi bi-layout-sidebar-inset";
            }
            topbarToggleBtn.title = isCollapsed ? "Mở rộng thanh menu" : "Thu gọn thanh menu";
        }

        if (brandPill) {
            brandPill.title = isCollapsed ? "Bấm vào để mở rộng menu" : "Trang quản trị SoftDev";
        }
    }

    function toggleAdminMenu() {
        if (!sidebar) return;
        sidebar.classList.toggle("collapsed");
        const isCollapsed = sidebar.classList.contains("collapsed");
        localStorage.setItem("softdev_sidebar_collapsed", isCollapsed ? "true" : "false");
        updateSidebarIcons(isCollapsed);
    }

    if (toggleBtn) {
        toggleBtn.addEventListener("click", function (e) {
            e.stopPropagation();
            toggleAdminMenu();
        });
    }

    if (topbarToggleBtn) {
        topbarToggleBtn.addEventListener("click", function (e) {
            e.stopPropagation();
            toggleAdminMenu();
        });
    }

    if (brandPill) {
        brandPill.addEventListener("click", function (e) {
            if (sidebar && sidebar.classList.contains("collapsed")) {
                e.preventDefault();
                toggleAdminMenu();
            }
        });
    }

    // Khởi tạo trạng thái thu gọn từ localStorage
    if (sidebar) {
        const isCollapsedSaved = localStorage.getItem("softdev_sidebar_collapsed") === "true";
        if (isCollapsedSaved) {
            sidebar.classList.add("collapsed");
        } else {
            sidebar.classList.remove("collapsed");
        }
        updateSidebarIcons(isCollapsedSaved);
    }

    // Phím tắt bàn phím: Ctrl + B hoặc Ctrl + \ để mở/thu gọn Sidebar
    document.addEventListener("keydown", function (e) {
        if ((e.ctrlKey || e.metaKey) && (e.key.toLowerCase() === "b" || e.key === "\\")) {
            e.preventDefault();
            toggleAdminMenu();
        }
    });
});

