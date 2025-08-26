import { apiInstance } from './core/api.module.js';
import { mvcInstance } from './core/mvc.module.js';
import { configInstance } from './core/config.module.js';
import { messageInstance } from './core/messages.module.js';
import { spinnerInstance } from './core/spinner.module.js';
import { broadCastInstance } from './core/broadcast.module.js';
import { dialogInstance } from './core/dialog.module.js';
import { utilities } from './core/utilities.module.js';

class CategoryModule {
    constructor() {
        this.configApp = configInstance;
        this.spinner = spinnerInstance;
        this.messageApp = messageInstance;
        this.token = this.configApp.token;
        this.selectedRow = null;
        this.selectedData = null;
        this.currentPage = 1;
        this.pageSize = 10;
        this.sortField = 'name';
        this.sortDirection = 'asc';
        this.globalData = [];
        this.totalCount = 0;

        this.form = null;
        this.modalCategory = null;
        this.tableId = "categoryTable";
        this.tableInstance = null;
    }

    debounce(func, delay) {
        let timeout;
        return function (...args) {
            clearTimeout(timeout);
            timeout = setTimeout(() => func.apply(this, args), delay);
        };
    }
    // init DataTable
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
                    sortField: d.order.length > 0 ? d.columns[d.order[0].column].data : "name",
                    sortDirection: d.order.length > 0 ? d.order[0].dir : "desc"
                };

                apiInstance.get('/Category/all', params)
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
                        console.error("❌ Error loading categories:", error);
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
                    title: "Category Name",
                    className: "fw-bold text-primary",
                    orderable: true
                },
                {
                    data: "slug",
                    title: "Slug",
                    className: "text-center",
                    orderable: true,
                    //render: function (data) {
                    //    if (!data) return "";
                    //    return moment(data).format("DD/MM/YYYY HH:mm:ss");
                    //}
                },
                {
                    data: null, // null để lấy toàn bộ row
                    title: "Parent name",
                    className: "text-center",
                    orderable: false,
                    render: function (data, type, row) {
                        if (data.parent && data.parent.name) {
                            return data.parent.name;
                        } else {
                            return '----';
                        }
                    }
                },
                {
                    data: null, // null để lấy toàn bộ row
                    title: "Status",
                    className: "text-center",
                    orderable: false,
                    render: function (data, type, row) {
                        if (data.isActive) {
                            return '<span class="badge bg-success">Active</span>';
                        } else {
                            return '<span class="badge bg-secondary">Deactivated</span>';
                        }
                    }
                },
                {
                    data: null, // null để lấy toàn bộ row
                    title: "Actions",
                    className: "text-center",
                    orderable: false,
                    render: function (data, type, row) {
                        return `<button class="btn btn-sm btn-warning btn-edit"
                                    onclick="categoryInstance.edit('${row.id}', '${row.name}')"
                                    data-id="${row.id}"
                                    data-name="${row.name}">Edit</button>
                                <button class="btn btn-sm btn-danger btn-delete"
                                    onclick="categoryInstance.delete('${row.id}', '${row.name}')"
                                    data-id="${row.id}"
                                    data-name="${row.name}">Delete</button>
                                `;
                    }
                }
            ],
            pageLength: 10
        });

    }
    
    save() {
        if (!this.validate()) return;
        const form = document.getElementById('form_add_category');
        // Tạo FormData từ form
        let payload = {
            id: form.querySelector('[name="Id"]').value || null,
            name: form.querySelector('[name="Name"]').value,
            slug: form.querySelector('[name="Slug"]').value,
            description: form.querySelector('[name="Description"]').value,
            parentId: form.querySelector('[name="ParentId"]').value || null,
            seoMetadataId: form.querySelector('[name="SeoMetadataId"]').value || null,
            seoMetadata: {
                title: form.querySelector('[name="SeoMetadata.Title"]').value,
                description: form.querySelector('[name="SeoMetadata.Description"]').value,
                keywords: form.querySelector('[name="SeoMetadata.Keywords"]').value
            }
        };
        payload.parentId = payload.parentId === "0" ? null : payload.parentId;
        apiInstance.post('/category/create', payload)
            .then(res => {
                messageInstance.success("Thêm mới thành công!");
                this.modalCategory.hide();
                this.refreshData();
            })
            .catch(err => {
                console.error(err);
                messageInstance.error("Lỗi khi thêm mới!");
            });
    }

    update() {
        if (!this.validate()) return;
        const form = document.getElementById('form_add_category');
        // Tạo FormData từ form
        let payload = {
            id: form.querySelector('[name="Id"]').value || null,
            name: form.querySelector('[name="Name"]').value,
            slug: form.querySelector('[name="Slug"]').value,
            description: form.querySelector('[name="Description"]').value,
            parentId: form.querySelector('[name="ParentId"]').value || null,
            seoMetadataId: form.querySelector('[name="SeoMetadataId"]').value || null,
            seoMetadata: {
                title: form.querySelector('[name="SeoMetadata.Title"]').value,
                description: form.querySelector('[name="SeoMetadata.Description"]').value,
                keywords: form.querySelector('[name="SeoMetadata.Keywords"]').value
            }
        };
        payload.parentId = payload.parentId === "0" ? null : payload.parentId;
        apiInstance.put('/category/update', payload)
            .then(res => {
                messageInstance.success("Cập nhật thành công!");
                this.modalCategory.hide();
                this.refreshData();
            })
            .catch(err => {
                console.error(err);
                messageInstance.error("Lỗi khi cập nhật!");
            });
    }
    add() {
        this.modalCategory = this.configApp.createModal("categoryModal");
        $('#categoryBody').html(''); // Xóa nội dung cũ trong dialog
        // set title    
        $('#categoryTitle').text('Thêm mới danh mục');
        mvcInstance.get('/Category/Create')
            .then(html => {
                $('#categoryBody').html(html);
                // set button create show, hide update
                $('.btn-update').hide();
                $('.btn-create').show();
                this.form = configInstance.initValidatorForm("categoryBody");
                this.modalCategory.show();
            }).catch((err) => {
                console.error('Lỗi tải form thêm mới:', err);
                messageInstance.error('Không thể tải form thêm mới');
            });
        
    }
   
    validate() {
        return this.form?.valid();
    }
    edit(id, name) {
        console.log("id = ", id);
        this.modalCategory = this.configApp.createModal("categoryModal");
        $('#categoryBody').html(''); // Xóa nội dung cũ trong dialog
        // set title    
        $('#categoryTitle').text('Cập nhật danh mục');
        mvcInstance.get('/Category/Edit/' + id)
            .then(html => {
                $('#categoryBody').html(html);
                $('.btn-update').show();
                $('.btn-create').hide();
                this.form = configInstance.initValidatorForm("categoryBody");
                this.modalCategory.show();
            }).catch((err) => {
                console.error('Lỗi tải form thêm mới:', err);
                messageInstance.error('Không thể tải form thêm mới');
            });

    }
    delete(id, name) {
        var message = "Bạn có chắc chắn muốn xóa danh mục <strong>" + name + "</strong> không?";
        this.messageApp.confirm(message).then((result) => {
            if (result) {
                apiInstance.delete('/category/delete?id=' + id)
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

export const categoryInstance = new CategoryModule(); 

$(document).change('#category-name', function () {
    $('#category-slug').val(utilities.slugify($('#category-name').val()));
});
// init modal on show
$(document).on('shown.bs.modal', '#categoryModal', function () {
    // nếu select2 đã được gắn trước đó thì destroy để tránh xung đột
    if ($('#ParentCategorySelect').hasClass("select2-hidden-accessible")) {
        $('#ParentCategorySelect').select2('destroy');
    }
    $('#ParentCategorySelect').select2({
        placeholder: "-- None --",
        allowClear: true,
        width: '100%',
        minimumResultsForSearch: 0,
        dropdownParent: $('#categoryModal')
    });

    // Auto select nếu chỉ có 1 option ngoài None
    var options = $('#ParentCategorySelect option').length;
    if (options === 2) {
        $('#ParentCategorySelect').prop('selectedIndex', 1).trigger('change');
    }
});
// Khi đóng modal -> destroy Select2 (tránh init chồng nhiều lần)
$(document).on('hidden.bs.modal', '#categoryModal', function () {
    if ($('#ParentCategorySelect').hasClass("select2-hidden-accessible")) {
        $('#ParentCategorySelect').select2('destroy');
    }
    $('#ParentCategorySelect').val(null); // reset về None
});
