
document.addEventListener('DOMContentLoaded', () => {
    // Lấy fullName từ ConfigModule
    const fullName = configInstance.user.fullName || ConfigModule.user.username || 'Người dùng';

    // Tìm tất cả các phần tử có class .aol_fullname và thay nội dung
    document.querySelectorAll('.aol_fullname').forEach(el => {
        el.textContent = fullName;
    });
});
