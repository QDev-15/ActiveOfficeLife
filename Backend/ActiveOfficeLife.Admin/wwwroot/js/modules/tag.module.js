import { apiInstance } from './core/api.module.js';
import { mvcInstance } from './core/mvc.module.js';
import { configInstance } from './core/config.module.js';
import { messageInstance } from './core/messages.module.js';
import { spinnerInstance } from './core/spinner.module.js';
import { broadCastInstance } from './core/broadcast.module.js';
import { dialogInstance } from './core/dialog.module.js';
import { utilities } from './core/utilities.module.js';

class TagModule {
    constructor() {
        this.configApp = configInstance;
        this.spinner = spinnerInstance;
        this.messageApp = messageInstance;
        this.token = configInstance.token;
        this.currentPage = 1;
        this.pageSize = 10;
        this.sortField = 'name';
        this.sortDirection = 'asc';
        this.globalData = [];
        this.totalCount = 0;
        
        this.form = null;
        this.tableId = "tagTable";
        this.tableInstance = null;
        this.EndPoints = {
            getAll: '/Tag/all',
            getById: '/Tag/getbyid',
            create: '/Tag/create',
            update: '/Tag/update',
            delete: '/Tag/delete'
        }
    }

    debounce(func, delay) {
        let timeout;
        return function (...args) {
            clearTimeout(timeout);
            timeout = setTimeout(() => func.apply(this, args), delay);
        };
    }
    // init Tag DataTable
    initTable() {
        this.tableInstance = $(`#${this.tableId}`).DataTable({
            processing: true,
            serverSide: true,
            stateSave: true,
            responsive: true,
            searchDelay: 500, // searchDelay mặc định sau 0.5s mới search
            order: [[1, 'desc']],
            pagingType: "full_numbers", // để hiển thị First, Prev, Next, Last
            mark: true, // ✅ Bật highlight search
            language: {
                paginate: {
                    first: '<i class="bi bi-chevron-double-left"></i>',
                    previous: '<i class="bi bi-chevron-left"></i>',
                    next: '<i class="bi bi-chevron-right"></i>',
                    last: '<i class="bi bi-chevron-double-right"></i>'
                }
            },
            ajax: (d, callback) => {
                let params = {
                    pageIndex: (d.start / d.length) + 1,
                    pageSize: d.length,
                    keySearch: d.search.value || "",
                    sortField: d.order.length > 0 ? d.columns[d.order[0].column].name : "name",
                    sortDirection: d.order.length > 0 ? d.order[0].dir : "desc"
                };

                apiInstance.get(this.EndPoints.getAll, params)
                    .then(response => {
                        this.pageSize = response.pageSize;
                        this.pageIndex = response.pageIndex;
                        callback({
                            data: response.items || [],
                            recordsTotal: response.totalCount || 0,
                            recordsFiltered: response.totalCount || 0,
                        });
                    })
                    .catch(error => {
                        console.error("❌ Error loading tags:", error);
                        callback({
                            data: [],
                            recordsTotal: 0,
                            recordsFiltered: 0
                        });
                    });
            },
            columns: [
                {
                    data: null,
                    title: "STT",
                    className: "text-center",
                    width: "60px",
                    render: function (data, type, row, meta) {
                        return meta.row + 1 + meta.settings._iDisplayStart;
                    },
                    orderable: false // Không sort theo STT
                },
                {
                    data: "name",
                    name: "name",
                    title: "Tag Name",
                    className: "fw-bold text-primary",
                    orderable: true
                },
                {
                    data: "slug",
                    name: "slug",
                    title: "Slug",
                    className: "text-center",
                    orderable: true
                },
                {
                    data: null, // null để lấy toàn bộ row
                    title: "Actions",
                    className: "text-center",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-sm btn-warning btn-edit"
                                    onclick="tagInstance.edit('${row.id}', '${row.name}')"
                                    data-id="${row.id}"
                                    data-name="${row.name}">Edit</button>
                                <button class="btn btn-sm btn-danger btn-delete"
                                    onclick="tagInstance.delete('${row.id}', '${row.name}')"
                                    data-id="${row.id}"
                                    data-name="${row.name}">Delete</button>
                                `;
                    }
                }
            ],
            pageLength: 10
        });

    }
    async addNew() {
        const tag = await apiInstance.post(this.EndPoints.create, {});
        // set tag id to modal input tagIdModal
        $('#tagIdModal').val(tag.id);
        this.edit(tag.id);
    }
    save() {
        if (!this.validate()) return;
        const form = document.getElementById('tag-form');
        // Tạo FormData từ form tag
        let payload = {
            id: form.querySelector('[name="Id"]').value || null,
            name: form.querySelector('[name="Name"]').value,
            slug: form.querySelector('[name="Slug"]').value,
            seoMetadataId: form.querySelector('[name="SeoMetadataId"]').value || null,
            seoMetadata: {
                metaTitle: form.querySelector('[name="SeoMetadata.MetaTitle"]').value,
                metaDescription: form.querySelector('[name="SeoMetadata.MetaDescription"]').value,
                metaKeywords: form.querySelector('[name="SeoMetadata.MetaKeywords"]').value
            }
        };
        apiInstance.put(this.EndPoints.update, payload)
            .then(res => {
                messageInstance.success("Cập nhập thành công!");
                this.modalTag.hide();
                this.refreshData();
            })
            .catch(err => {
                console.error(err);
                messageInstance.error("Lỗi khi thêm mới!");
            });
    }

    validate() {
        return this.form?.valid();
    }
    edit(id, name) {
        console.log("id = ", id);
        this.modalTag = this.configApp.createModal("tagModal");
        $('#tagBody').html(''); // Xóa nội dung cũ trong dialog
        mvcInstance.get('/Tag/TagForm/' + id)
            .then(html => {
                $('#tagBody').html(html);
                $('.btn-create').show();
                this.form = configInstance.initValidatorForm("tagBody");
                this.form?.valid();
                this.modalTag.show();
                // auto focus to input name
                setTimeout(() => {
                    $('#Name').focus();
                    const el = document.getElementById('metaKeywords');
                    if (el) utilities.initTags(el);
                }, 500);
            }).catch((err) => {
                console.error('Lỗi tải form thêm mới:', err);
                messageInstance.error('Không thể tải form thêm mới');
            });

    }
    delete(id, name) {
        var message = "Bạn có chắc chắn muốn xóa danh mục <strong>" + name + "</strong> không?";
        this.messageApp.confirm(message).then((result) => {
            if (result) {
                apiInstance.delete(this.EndPoints.delete +'?id=' + id)
                    .then(res => {
                        this.messageApp.success("Xóa thành công!");
                        this.refreshData();
                    })
                    .catch(err => {
                        console.error(err);
                        this.messageApp.error("Lỗi khi xóa danh mục!");
                    });
            };
        });
    }

    refreshData() {
        this.tableInstance?.ajax.reload();
    }

}

export const tagInstance = new TagModule(); 
