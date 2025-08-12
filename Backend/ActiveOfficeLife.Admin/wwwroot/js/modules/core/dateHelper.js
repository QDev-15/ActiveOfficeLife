

class DateHelper {
    formatDefault(dateString) {
        const format = "DD/MM/YYYY HH:mm:ss"
        if (!dateString) return "";
        try {
            return dayjs(dateString).format(format);
        } catch (e) {
            console.warn("Lỗi format createdAt:", e);
            return dateString;
        }
    }
    format(dateString, formatDate) {
        const format = formatDate ?? "DD/MM/YYYY HH:mm:ss"
        if (!this.user?.createdAt) return "";
        try {
            return dayjs(dateString).format(format);
        } catch (e) {
            console.warn("Lỗi format createdAt:", e);
            return dateString;
        }
    }
}

export const dateHelper = new DateHelper();