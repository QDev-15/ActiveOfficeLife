// wwwroot/js/config.js
import { dateHelper } from "./dateHelper.js";
class ConfigModule {
    constructor() {
        this.token = document.cookie
            .split('; ')
            .find(row => row.startsWith('AccessToken='))
            ?.split('=')[1] || '';

        this.urlApiServer = 'https://api.aol.tkid.io.vn/api';
        this.urlApiLocal = 'https://localhost:7029/api';
        this.urlApi = this.urlApiServer;

        this.user = {
            id: null,
            username: '',
            fullName: '',
            email: '',
            avatarUrl: '',
            phoneNumber: '',
            status: null,
            roles: [],
            createdAt: null
        };

        this.initUserCookie();
    }

    createModal(id) {
        const modalEl = document.getElementById(id);
        return new bootstrap.Modal(modalEl);
    }
    initUserCookie() {
        if (!this.token) {
            this.logout();
        }
        var userInfo = document.cookie
            .split('; ')
            .find(row => row.startsWith('userinfo='))
            ?.split('=')[1] || '';
        if (userInfo) {
            try {
                // decodeURIComponent vì cookie bị encode
                const decoded = decodeURIComponent(userInfo);
                const userData = JSON.parse(decoded);

                this.user.id = userData.Id || userData.id || null;
                this.user.username = userData.Username || userData.username || '';
                this.user.fullName = userData.FullName || userData.fullName || this.user.username;
                this.user.email = userData.Email || '';
                this.user.avatarUrl = userData.AvatarUrl || '';
                this.user.phoneNumber = userData.PhoneNumber || '';
                this.user.status = userData.Status ?? null;
                this.user.createdAt = userData.CreatedAt || null;

                this.user.createdAt = dateHelper.formatDefault(this.user.createdAt);
                // Roles xử lý an toàn
                let rawRoles = userData.Roles || userData.roles || [];
                if (typeof rawRoles === 'string') {
                    rawRoles = rawRoles.split(',').map(r => r.trim());
                }
                if (Array.isArray(rawRoles)) {
                    this.user.roles = rawRoles;
                } else {
                    this.user.roles = [];
                }
                this.updateUI();

            } catch (err) {
                console.warn('Không thể parse userinfo cookie:', err);
            }
        }
    }
    checkLogined() {
        const token = document.cookie
            .split('; ')
            .find(row => row.startsWith('AccessToken='))
            ?.split('=')[1] || '';
        if (!token) {
            this.logout();
        }
    }
    updateUI() {
        // Thay toàn bộ .aol_fullname bằng tên user
        if (this.user?.fullName) {
            document.querySelectorAll('.aol_fullname').forEach(el => {
                el.textContent = this.user.fullName;
            });
        }
    }
    
    clear() {
        this.user = null;
        this.token = null;
        this.updateUI();
    }

    logout() {
        this.clear();
        // Điều hướng sang trang login
        window.location.href = '/login';
    }
    setUser(user) { this.user = user; }
    setToken(token) { this.token = token; }
    getUsername() { return this.user.username; }
    getFullname() { return this.user.fullname; }
    hasRole(role) { return this.user.roles.includes(role); }
    getToken() { return this.token; }
    initValidatorForm(formId) {
        let $form = $("#" + formId).find("form");
        if ($form.length) {
            $.validator.unobtrusive.parse($form);
        }
        return $form;
    }
}

export const configInstance = new ConfigModule();


