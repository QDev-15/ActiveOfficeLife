import { apiInstance } from './core/api.module.js';
import { mvcInstance } from './core/mvc.module.js';
import { configInstance } from './core/config.module.js';
import { messageInstance } from './core/messages.module.js';
import { spinnerInstance } from './core/spinner.module.js';
import { broadCastInstance } from './core/broadcast.module.js';
import { dialogInstance } from './core/dialog.module.js';

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
    }

    fetchData(sortField = 'name', sortDirection = 'asc', pageIndex = 1, pageSize = 10) {
        this.spinner.showFor("table-category");
        apiInstance.get('/Category/all-paging', {
            sortField: sortField,
            sortDirection: sortDirection,
            pageIndex: pageIndex,
            pageSize: pageSize
        }).then(result => {
            this.globalData = result.items || result;
            this.totalCount = result.totalCount || this.globalData.length;
            this.renderTable(this.globalData);
            this.renderPagination(this.totalCount);
            this.spinner.hideFor("table-category");
        }).catch(error => {
            this.showError('Không thể tải dữ liệu');
            throw new Error(`Lỗi ${error.status}: ${error.statusText}`);
        }).finally(() => {
            this.spinner.hideFor("table-category");
        });
    }

    renderTable(data) {
        const tbody = document.getElementById('userTableBody');
        tbody.innerHTML = '';

        data.forEach((cate, index) => {
            const row = document.createElement('tr');
            row.innerHTML = `
              <td>${(this.currentPage - 1) * this.pageSize + index + 1}</td>
              <td>${cate.name}</td>
              <td>${cate.slug}</td>
              <td>
                <button class="btn btn-success me-2" data-id="${cate.id}">✏️ Edit</button>
              </td>
            `;

            row.onclick = () => {
                if (this.selectedRow) this.selectedRow.classList.remove('selected');
                row.classList.add('selected');
                this.selectedRow = row;
                this.selectedData = cate;
            };

            const btn = row.querySelector('button');
            btn.addEventListener('click', (event) => {
                event.stopPropagation();
                this.editUser();
            });

            tbody.appendChild(row);
        });
    }

    renderPagination(totalItems) {
        const totalPages = Math.ceil(totalItems / this.pageSize);
        const container = document.getElementById('pagination');
        container.innerHTML = '';

        const prevBtn = document.createElement('li');
        prevBtn.className = `page-item ${this.currentPage === 1 ? 'disabled' : ''}`;
        prevBtn.innerHTML = `<a class="page-link" href="#">«</a>`;
        prevBtn.onclick = () => this.changePage(this.currentPage - 1);
        container.appendChild(prevBtn);

        for (let i = 1; i <= totalPages; i++) {
            const pageBtn = document.createElement('li');
            pageBtn.className = `page-item ${i === this.currentPage ? 'active' : ''}`;
            pageBtn.innerHTML = `<a class="page-link" href="#">${i}</a>`;
            pageBtn.onclick = () => this.changePage(i);
            container.appendChild(pageBtn);
        }

        const nextBtn = document.createElement('li');
        nextBtn.className = `page-item ${this.currentPage === totalPages ? 'disabled' : ''}`;
        nextBtn.innerHTML = `<a class="page-link" href="#">»</a>`;
        nextBtn.onclick = () => this.changePage(this.currentPage + 1);
        container.appendChild(nextBtn);
    }

    changePage(page) {
        this.currentPage = page;
        this.fetchData(this.sortField, this.sortDirection, this.currentPage, this.pageSize);
    }

    onSort(field) {
        if (this.sortField === field) {
            this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
        } else {
            this.sortField = field;
            this.sortDirection = 'asc';
        }
        this.currentPage = 1;
        this.fetchData(this.sortField, this.sortDirection, this.currentPage, this.pageSize);
    }

    save(payload) {
        apiInstance.post('/category/create', payload)
            .then(res => {
                messageInstance.info("Thêm mới thành công!");
                this.fetchData();
            })
            .catch(err => {
                console.error(err);
                messageInstance.error("Lỗi khi thêm mới!");
            });
        // Xử lý lưu dữ liệu ở đây
        this.messageApp.info('Đã lưu người dùng mới');
        this.fetchData();
    }
    addCategory() {
        mvcInstance.get('/Category/Create')
            .then(html => {
                dialogInstance.open({
                    title: 'Thêm mới',
                    bodyHtml: html,
                    showSave: true,
                    saveText: 'Lưu',
                    showClose: true,
                    onResult: (result) => {
                        if (result === 'save') {
                            // Lấy form
                            const form = document.getElementById('add_category');

                            // Tạo FormData từ form
                            let payload = {
                                name: form.querySelector('[name="Name"]').value,
                                slug: form.querySelector('[name="Slug"]').value,
                                description: form.querySelector('[name="Description"]').value,
                                parentId: form.querySelector('[name="ParentId"]').value || null,
                                seoMetadata: {
                                    title: form.querySelector('[name="SeoMetadata.Title"]').value,
                                    description: form.querySelector('[name="SeoMetadata.Description"]').value,
                                    keywords: form.querySelector('[name="SeoMetadata.Keywords"]').value
                                }
                            };
                            this.save(payload);
                        }
                    },
                    onViewInited: function (dialogBodyEl) {
                        $.validator.unobtrusive.parse($(dialogBodyEl).find('form'));
                        // Khởi tạo Select2 với filter
                        $('#ParentCategorySelect').select2({
                            placeholder: "-- None --",
                            allowClear: true,
                            width: '100%'
                        });

                        // Auto select nếu chỉ có 1 option (ngoài None)
                        var options = $('#ParentCategorySelect option').length;
                        if (options === 2) { // None + 1 category
                            $('#ParentCategorySelect').prop('selectedIndex', 1).trigger('change');
                        }
                    }
                });
            }).catch((err) => {
                console.error('Lỗi tải form thêm mới:', err);
                messageInstance.error('Không thể tải form thêm mới');
            });
    }

    editUser() {
        if (!this.selectedData) return alert('Vui lòng chọn dòng để sửa!');
        this.messageApp.info('Sửa người dùng: ' + this.selectedData.name);
    }

    initSelect2() {
        // Khởi tạo Select2 với filter
        $('#ParentCategorySelect').select2({
            placeholder: "-- None --",
            allowClear: true,
            width: '100%'
        });

        // Auto select nếu chỉ có 1 option (ngoài None)
        var options = $('#ParentCategorySelect option').length;
        if (options === 2) { // None + 1 category
            $('#ParentCategorySelect').prop('selectedIndex', 1).trigger('change');
        }
    }
    refreshData() {
        this.fetchData();
        //this.messageApp.confirm("Bạn có muốn tiếp tục không?")
        //    .then(result => {
        //        if (result) {
        //            this.fetchData();
        //        } else {
        //            console.log("Người dùng đã chọn KHÔNG");
        //        }
        //    });
    }

    showError(message) {
        this.messageApp.info(message); // bạn có thể thay bằng Message.error nếu muốn
    }
}

export const categoryInstance = new CategoryModule();

