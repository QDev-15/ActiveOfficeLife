

class Utilities {
    slugify(text) {
        if (!text) return '';
        return text
            .toString()
            .replace(/Đ/g, 'D')
            .replace(/đ/g, 'd')
            .normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '')
            .toLowerCase()
            .trim()
            .replace(/[^a-z0-9\s-]/g, '')
            .replace(/\s+/g, '-')
            .replace(/-+/g, '-');
    }

}
export const utilities = new Utilities();