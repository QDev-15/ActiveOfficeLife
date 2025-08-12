// wwwroot/js/dialogModule.js
class DialogModule {
    constructor() {
        this.modalEl = document.getElementById("appDialog");
        this.modal = new bootstrap.Modal(this.modalEl);
        this.resultCallback = null;
    }

    open(config) {
        // Cấu hình mặc định
        const defaultConfig = {
            title: "",
            icon: "",
            bodyId: "",
            bodyHtml: "",
            showClose: true,
            showSave: false,
            saveText: "Save",
            showUpdate: false,
            updateText: "Update",
            onSave: null,
            onUpdate: null
        };

        // Merge config truyền vào với default
        const finalConfig = { ...defaultConfig, ...config };

        config = finalConfig;
        // Set title & icon
        document.getElementById("dialogTitle").innerText = config.title || "";
        document.getElementById("dialogIcon").innerHTML = config.icon || "";

        // Set body content
        if (config.bodyId) {
            let bodyHtml = document.getElementById(config.bodyId)?.innerHTML || "";
            document.getElementById("dialogBody").innerHTML = bodyHtml;
        } else if (config.bodyHtml) {
            document.getElementById("dialogBody").innerHTML = config.bodyHtml;
        }

        // Set footer buttons
        let footerEl = document.getElementById("dialogFooter");
        footerEl.innerHTML = "";

        if (config.showClose !== false) {
            let btnClose = document.createElement("button");
            btnClose.type = "button";
            btnClose.className = "btn btn-secondary";
            btnClose.innerText = "Close";
            btnClose.addEventListener("click", () => this.close());
            footerEl.appendChild(btnClose);
        }

        if (config.showSave) {
            let btnSave = document.createElement("button");
            btnSave.type = "button";
            btnSave.className = "btn btn-primary";
            btnSave.innerText = config.saveText || "Save";
            btnSave.addEventListener("click", () => {
                if (this.resultCallback) this.resultCallback("save");
                this.close();
            });
            footerEl.appendChild(btnSave);
        }

        if (config.showUpdate) {
            let btnUpdate = document.createElement("button");
            btnUpdate.type = "button";
            btnUpdate.className = "btn btn-warning";
            btnUpdate.innerText = config.updateText || "Update";
            btnUpdate.addEventListener("click", () => {
                if (this.resultCallback) this.resultCallback("update");
                this.close();
            });
            footerEl.appendChild(btnUpdate);
        }

        // Save callback
        this.resultCallback = config.onResult || null;

        // Show modal
        this.modal.show();
    }

    close() {
        this.modal.hide();
    }
}

export const dialogInstance = new DialogModule();
