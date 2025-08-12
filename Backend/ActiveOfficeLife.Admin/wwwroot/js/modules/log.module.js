import { configInstance } from "./core/config.module.js";
import { messageInstance } from "./core/messages.module.js";
import { apiInstance } from "./core/api.module.js";
import { spinnerInstance } from "./core/spinner.module.js"
import { dateHelper } from "./core/dateHelper.js"; 


class LogModule {
    constructor() {
        this.config = configInstance;
        this.messageApp = messageInstance;
        this.currentPage = 1;
        this.pageIndex = 1;
        this.pageSize = 10;
        this.globalData = [];
        this.totalCount = 0;
    }

    fetchData(startDate, endDate, pageIndex = 1, pageSize = 10) {

        spinnerInstance.showFor("table-log");
        apiInstance.get('/loger/all', {
            startDate: startDate,
            endDate: endDate,
            pageIndex: pageIndex,
            pageSize: pageSize
        }).then((res) => {
            this.globalData = res.items || res;         // Giả sử API trả về { data: { items: [], totalCount: 123 } }
            this.totalCount = res.totalCount || globalData.length;
            this.renderTable(this.globalData);
            this.renderPagination(this.totalCount);
        }).catch((err) => {
            console.error('Lỗi gọi API:', err.message);
            this.messageApp.error('Không thể tải dữ liệu');
        }).finally(() => {
            spinnerInstance.hideFor("table-log");
        });
    }
    renderTable(data) {
        const tbody = document.getElementById('logTableBody');
        tbody.innerHTML = '';

        data.forEach((log, index) => {
            const row = document.createElement('tr');
            row.innerHTML = `
              <td>${(this.currentPage - 1) * this.pageSize + index + 1}</td>
              <td class="col-message">${log.message}</td>
              <td>${log.level}</td>
              <td>${dateHelper.formatDefault(log.timestamp)}</td >
              <td>
                <button class="btn btn-success me-2" data-id="${log.id}">✏️ Edit</button>
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
        this.fetchData(null, null, this.currentPage, this.pageSize);
    }
}

export const logInstance = new LogModule();