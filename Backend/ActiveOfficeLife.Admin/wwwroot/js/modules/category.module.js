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
        this.token = configInstance.token;
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
        this.categories = [];   
        this.categorytypes = [];
        this.tableId = "categoryTable";
        this.tableCategoryTypeId = "categoryCategoryTable";
        this.tableInstance = null;
        this.tableInstanceCategoryType = null;
        this.endpoints = {
            create: '/Category/create',
            update: '/Category/update',
            delete: (id) => `/Category/delete?id=${id}`, 
            getall: '/Category/all',
            createType: '/Category/create-type',
            getTypes: '/Category/types',
            updateType: '/Category/update-type',
            deleteType: (id) => `/Category/delete-type?id=${id}`
        }
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
                    sortField: d.order.length > 0 ? d.columns[d.order[0].column].name : "type",
                    sortDirection: d.order.length > 0 ? d.order[0].dir : "desc"
                };

                apiInstance.get(this.endpoints.getall, params)
                    .then(response => {
                        this.pageSize = response.pageSize;
                        this.pageIndex = response.pageIndex;
                        this.categories = response.items || [];
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
                    name: "name",
                    title: "Category Name",
                    className: "fw-bold text-primary",
                    orderable: true
                },
                {
                    data: "slug",
                    name: "slug",
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
                    name: "type",
                    title: "Types",
                    className: "text-center",
                    orderable: true,
                    render: function (data, type, row) {
                        if (data.categoryType) {
                            return row.categoryType.name
                        }
                        return '';
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
        this.initSearchCategoryDebounce();
    }
    initSearchCategoryDebounce() {
        // ID ô search mặc định của DataTables: <tableId>_filter input
        const $searchInput = $(`#${this.tableId}_filter input`);

        // Gỡ handler mặc định của DataTables
        $searchInput.off('keyup.DT input.DT');

        let typingTimer = null;
        const delay = 500; // 500ms

        $searchInput.on('keyup input', (e) => {
            const value = e.target.value;

            clearTimeout(typingTimer);

            typingTimer = setTimeout(() => {
                // Gọi search sau khi ngừng gõ 500ms
                this.tableInstance.search(value).draw();
            }, delay);
        });
    }
    // init DataTable
    initTableCategoryType() {
        this.tableInstanceCategoryType = $(`#${this.tableCategoryTypeId}`).DataTable({
            processing: true,
            serverSide: true,
            stateSave: true,
            responsive: true,
            searching: false,
            searchDelay: 500, // searchDelay mặc định sau 0.5s mới search
            order: [[1, 'desc']],
            ajax: (d, callback) => {
                apiInstance.get(this.endpoints.getTypes)
                    .then(response => {
                        this.pageSize = response.pageSize;
                        this.pageIndex = response.pageIndex;
                        this.categorytypes = response.items || [];
                        this.refreshData();
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
                    name: "name",
                    title: "Category Type Name",
                    className: "fw-bold text-primary",
                    orderable: true
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
                                    onclick="categoryInstance.editType(event)"
                                    data-id="${row.id}" data-active="${row.isActive}"
                                    data-name="${row.name}">Edit</button>
                                <button class="btn btn-sm btn-danger btn-delete"
                                    onclick="categoryInstance.deleteType(event)"
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
            parentId: form.querySelector('[name="ParentId"]')?.value || null,
            categoryTypeId: form.querySelector('[name="CategoryTypeId"]')?.value || null,
            seoMetadataId: form.querySelector('[name="SeoMetadataId"]')?.value || null,
            isActive: form.querySelector('[name="IsActive"]').checked,
            seoMetadata: {
                seoMetaTitle: form.querySelector('[name="SeoMetadata.Title"]').value,
                seoMetaDescription: form.querySelector('[name="SeoMetadata.Description"]').value,
                seoMetaKeywords: form.querySelector('[name="SeoMetadata.Keywords"]').value
            }
        };
        payload.parentId = payload.parentId === "0" ? null : payload.parentId;
        apiInstance.post(this.endpoints.create, payload)
            .then(res => {
                messageInstance.success("Thêm mới thành công!");
                this.modalCategory.hide();
                this.refreshData();
            })
            .catch(err => {
                console.error(err);
                var messageErr = err.message;
                messageInstance.error(messageErr);
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
            parentId: form.querySelector('[name="ParentId"]')?.value || null,
            categoryTypeId: form.querySelector('[name="CategoryTypeId"]')?.value || null,
            seoMetadataId: form.querySelector('[name="SeoMetadataId"]')?.value || null,
            isActive: form.querySelector('[name="IsActive"]').checked,
            seoMetadata: {
                title: form.querySelector('[name="SeoMetadata.Title"]').value,
                description: form.querySelector('[name="SeoMetadata.Description"]').value,
                keywords: form.querySelector('[name="SeoMetadata.Keywords"]').value
            }
        };
        payload.parentId = payload.parentId === "0" ? null : payload.parentId;
        apiInstance.put(this.endpoints.update, payload)
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
                spinnerInstance.showFor(this.tableId);
                apiInstance.delete(this.endpoints.delete(id))
                    .then(res => {
                        this.messageApp.success("Xóa thành công!");
                        this.refreshData();
                    })
                    .catch(err => {
                        console.error(err);
                        var messageErr = err.message;
                        messageInstance.error(messageErr);
                    })
                    .finally(() => {
                        spinnerInstance.hideFor(this.tableId);
                    });
            };
        });
    }

    addType() {
        // reset form data set new value for input typeName
        $('#typeName').val('new types');
        // reset hidden input typeId
        $('#typeId').val('');
        $('#typeIsActive').prop('checked', true);
        this.showEditFormType(true);
    }
    editType(e) {
        // get data-id and data-name from button
        const id = $(e.currentTarget).data('id');
        const name = $(e.currentTarget).data('name');
        const isActive = $(e.currentTarget).data('active');
        // set value for input typeName
        $('#typeName').val(name);
        // set value for hidden input typeId
        $('#typeId').val(id);
        $('#typeIsActive').prop('checked', isActive);
        this.showEditFormType(true);
    }
    saveType() {
        const type = {
            id: $('#typeId').val() || null,
            name: $('#typeName').val(),
            isActive: $('#typeIsActive').prop('checked')
        }
        if (!type.name || type.name.trim() === '') {
            this.messageApp.error("Vui lòng nhập tên loại danh mục!");
            return;
        }
        if (!type.id) {
            apiInstance.post(this.endpoints.createType, type)
                .then(res => {    
                    this.resetFormType();
                    this.refreshTypeData();
                    this.messageApp.success("Lưu loại danh mục thành công!");
                })
                .catch(err => {
                    console.error(err);
                    this.messageApp.error("Lỗi khi lưu loại danh mục!");
                });
        } else {
            apiInstance.put(this.endpoints.updateType, type)
                .then(res => {
                    this.resetFormType();
                    this.refreshTypeData();
                    this.messageApp.success("Cập nhật loại danh mục thành công!");
                })
                .catch(err => {
                    console.error(err);
                    this.messageApp.error("Lỗi khi cập nhật loại danh mục!");
                });
        }
    }
    showEditFormType(show) {
        // show or hide form <form id="category-type-form" style="display: none">
        if (show) {
            $('#category-type-form').show();
            $('#card-body-title').hide();
        } else {
            $('#category-type-form').hide();
            $('#card-body-title').show();
        }
    }
    resetFormType() {
        this.showEditFormType(false);
        // reset form <form id="category-type-form">
        $('#category-type-form')[0].reset();
    }
    deleteType(e) {
        // get data-id and data-name from button
        const id = $(e.currentTarget).data('id');
        const name = $(e.currentTarget).data('name');
        var message = "Bạn có chắc chắn muốn xóa loại danh mục <strong>" + name + "</strong> không?";
        this.messageApp.confirm(message).then((result) => {
            if (result) {
                apiInstance.delete(this.endpoints.deleteType(id))
                    .then(res => {
                        this.messageApp.success("Xóa loại danh mục thành công!");
                        this.refreshTypeData();
                    })
                    .catch(err => {
                        //console.error(err);
                        const messageError = err.message;
                        this.messageApp.error(messageError);
                    });
            };
        });
    }
    refreshData(cache) {
        if (cache) {
            this.tableInstance?.ajax.reload(null, false); // reload giữ nguyên trang hiện tại
            return;
        }
        this.tableInstance?.ajax.reload();
    }
    refreshTypeData() {
        this.refreshData(true);
        this.tableInstanceCategoryType?.ajax.reload();
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
