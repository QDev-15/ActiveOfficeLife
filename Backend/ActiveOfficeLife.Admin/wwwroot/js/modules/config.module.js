// wwwroot/js/config.js
export class ConfigModule {
    static token = document.cookie
        .split('; ')
        .find(row => row.startsWith('AccessToken='))
        ?.split('=')[1] || '';

    

    static urlApiServer = 'https://api.aol.tkid.io.vn/api';
    static urlApiLocal = 'https://localhost:7029/api';
    static urlApi = this.urlApiServer;

    static user = {
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

    static initUserCookie() {
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

                
            } catch (err) {
                console.warn('Không thể parse userinfo cookie:', err);
            }
        }
    }
    
    static setUser(user) {
        this.user = user;
    }

    static setToken(token) {
        this.token = token;
    }

    static getUsername() {
        return this.user.username;
    }

    static getFullname() {
        return this.user.fullname;
    }

    static hasRole(role) {
        return this.user.roles.includes(role);
    }

    static getToken() {
        return this.token;
    }
}




