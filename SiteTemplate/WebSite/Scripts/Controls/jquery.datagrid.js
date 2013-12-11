/// <reference path="jquery-1.9.1.min.js" />

!(function ($) {

    /* DataGrid PUBLIC CLASS DEFINITION
    * ============================== */

    var DataGrid = function (element, options) {

        this.$datagrid = $(element);
        this.$datagrid_columnTitle = this.$datagrid.find('thead > tr.column_title');
        this.$datagrid_tbody = this.$datagrid.find('tbody');
        this.$datagrid_paging = this.$datagrid.find('tfoot').find('td.paging');

        this.dataUrl = options.dataUrl;   // 数据的action
        this.pageSize = options.pageSize; // 分页大小
        this.pageIndex = 1;
        this.columnCount = this.$datagrid_columnTitle.find('th').length;   // 列数
        this.rowCount = 0;      // 行数

        this.updatePageInfo = function (page_index, record_total) {
            this.pageIndex = page_index;
            if (this.$datagrid_paging != null) {
                // 1.计算分页信息
                var pageCount = Math.ceil(record_total / this.pageSize);
                var lastPage = (page_index - 1) < 1 ? 1 : (page_index - 1);
                var nextPage = (+page_index + 1) > pageCount ? pageCount : (+page_index + 1);

                // 2.记录分页信息
                this.$datagrid_paging.attr('colspan', this.columnCount);
                this.$datagrid_paging.find('.page_size').text(this.pageSize);
                this.$datagrid_paging.find('.page_index').text(page_index);
                this.$datagrid_paging.find('.page_total').text(pageCount);
                this.$datagrid_paging.find('.record_total').text(record_total);
                this.$datagrid_paging.find('a.lastPage').attr('page', lastPage);
                this.$datagrid_paging.find('a.nextPage').attr('page', nextPage);
            }
        }

        this.updateSortInfo = function (sort_column, sort_direction) {
            //TODO 
        }

    }


    DataGrid.prototype = {

        constructor: DataGrid
    ,
        updateColumnTitle: function (columnTitles) {  // 更新列标题
            if (this.$datagrid_columnTitle != null) {
                var row_columnTitle = '';
                $.each(columnTitles, function (fieldName, displayName) {
                    var cell_content = '<th field="' + fieldName + '">' + displayName + '</th>';
                    row_columnTitle += cell_content;
                });

                this.$datagrid_columnTitle.html(row_columnTitle);

                // 更新columnCount
                this.columnCount = this.$datagrid_columnTitle.find('th').length;
            }
        }
    ,
        updateContent: function (records) {     // 更新表格内容
            if (this.$datagrid_columnTitle == null || this.$datagrid_tbody == null) return;

            var that = this;
            var tbody_content = '';
            that.rowCount = 0;
            $.each(records, function (rowIndex, rowData) {
                if (that.rowCount >= that.pageSize) return;

                // 按行组装记录
                var row_content = '<tr>';
                that.$datagrid_columnTitle.find('th').each(function (columnIndex, column) {

                    var columnValue = '';
                    var columnName = $(column).attr('field');
                    if (columnName != null) {
                        $.each(rowData, function (propertyName, propertyValue) {
                            if (columnName == propertyName) {
                                columnValue = propertyValue;
                            }
                        });
                    }

                    var cell_content = '<td>' + columnValue + '</td>';
                    row_content += cell_content;
                });
                row_content += '</tr>';

                tbody_content += row_content;
                that.rowCount++;
            });

            that.$datagrid_tbody.html(tbody_content);
        }
    ,
        initPagingEvent: function () {    // 1.绑定分页事件
            if (this.$datagrid_paging != null) {
                var that = this;
                this.$datagrid_paging.find('a.lastPage').on('click', function () {
                    var pageIndex = $(this).attr('page');
                    that.paging(pageIndex);
                });
                this.$datagrid_paging.find('a.nextPage').on('click', function () {
                    var pageIndex = $(this).attr('page');
                    that.paging(pageIndex);
                });
            }
        }
    ,
        initSortEvent: function () {    // 2.绑定排序事件

            // todo
        }
    ,
        refresh: function () {          // 刷新

        }
    ,
        paging: function (pageIndex) {  // 分页
            var that = this;
            $.getJSON(this.dataUrl, function (json) {
                // 获取数据
                var data = $.extend({}, { columnTitles: {}, records: [], record_total: 0 }, json);

                // 更新DataGrid
                that.updateColumnTitle(data.columnTitles);
                that.updateContent(data.records);
                that.updatePageInfo(pageIndex, data.record_total);
            });
        }
    ,
        sorting: function () {          // 排序
            // todo
        }
    }


    /* DataGrid PLUGIN DEFINITION
    * ========================== */

    var old = $.fn.dataGrid

    $.fn.dataGrid = function (option) {
        return this.each(function () {
            var $this = $(this)
              , data = $this.data('dataGrid')
              , options = $.extend({}, $.fn.dataGrid.defaults, typeof option == 'object' && option)
            if (!data) $this.data('dataGrid', (data = new DataGrid(this, options)));

            data.initPagingEvent();
            data.paging(1);
        });
    }

    $.fn.dataGrid.defaults = { pageSize: 10, dataUrl: null };

    $.fn.dataGrid.Constructor = DataGrid;


    /* DataGrid NO CONFLICT
    * =================== */

    $.fn.dataGrid.noConflict = function () {
        $.fn.dataGrid = old
        return this
    }

})(jQuery); 

