

class Utilities {
    slugify(text) {
        if (!text) return '';
        return text
            .toString()                    // ensure string
            .normalize('NFD')               // split accented chars
            .replace(/[\u0300-\u036f]/g, '')// remove accents
            .toLowerCase()                  // lowercase
            .trim()                         // remove leading/trailing spaces
            .replace(/[^a-z0-9\s-]/g, '')   // remove invalid chars
            .replace(/\s+/g, '-')           // collapse whitespace -> dash
            .replace(/-+/g, '-');           // collapse multiple dashes
    }

}
export const utilities = new Utilities();