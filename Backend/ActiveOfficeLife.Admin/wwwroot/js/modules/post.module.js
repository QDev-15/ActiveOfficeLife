import { apiInstance } from './core/api.module.js';
import { mvcInstance } from './core/mvc.module.js';
import { configInstance } from './core/config.module.js';
import { messageInstance } from './core/messages.module.js';
import { spinnerInstance } from './core/spinner.module.js';
import { broadCastInstance } from './core/broadcast.module.js';
import { dialogInstance } from './core/dialog.module.js';
import { utilities } from './core/utilities.module.js';
class PostModule {

    constructor() {
        this.token = configInstance.token;
        this.currentPage = 1;
        this.pageSize = 10;
        this.sortField = 'name';
        this.sortDirection = 'asc';
        this.globalData = [];
        this.totalCount = 0;
        this.tableId = "articlesTable";
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
                    status: $("#article_status").val(),
                    pageIndex: (d.start / d.length) + 1,
                    pageSize: d.length,
                    keySearch: d.search.value || "",
                    sortField: d.order.length > 0 ? d.columns[d.order[0].column].name : "name",
                    sortDirection: d.order.length > 0 ? d.order[0].dir : "desc"
                };

                apiInstance.get('/post/all', params)
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
                    data: "title",
                    name: "title",
                    title: "Article Name",
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
                    name: "category",
                    title: "Category name",
                    className: "text-center",
                    orderable: true,
                    render: function (data, type, row) {
                        if (data.category && data.category.name) {
                            return data.category.name;
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
                        if (data.status) {
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
}
export const postInstance = new PostModule(); 
