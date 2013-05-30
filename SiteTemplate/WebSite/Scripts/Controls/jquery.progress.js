/// <reference path="../jquery-1.9.1.min.js" />

!function ($) {

    /* ProgressBar PUBLIC CLASS DEFINITION
    * ============================== */

    var ProgressBar = function (container, options) {

        this.step = options.step;  // 步长
        this.interval = options.interval; // 每步的时间间隔ms
        this.maxPercent = options.maxPercent;  // 行进到最大百分比
        this.percentage = 0;
        this.timeOut = null;

        this.move = function () {
            var percent = this.percentage + this.step;
            if (percent <= this.maxPercent) {
                this.setPercentage(percent);
                var that = this;
                this.timeOut = setTimeout(function () {
                    that.move();
                }, that.interval);
            }
        }

        this.$container = $(container).css('position', 'relative');
        this.$element = $('<div class="modal-backdrop fade in" style="position:absolute; display:none;"><div class="progress" style="margin:' + (this.$container.height() / 2 - 10) + 'px 10%;"><div class="bar"></div></div></div>').appendTo(this.$container);
        this.$bar = this.$element.find('div.bar');

    }

    ProgressBar.prototype = {

        constructor: ProgressBar
    ,
        getPercentage: function () {
            return this.percentage;
        }
    ,
        setPercentage: function (percent) {
            this.percentage = percent;
            var showPercent = Math.round(this.percentage) + '%';
            this.$bar.width(showPercent).text(showPercent);
        }
    ,
        setText: function (percent, text) {
            this.percentage = percent;
            var showPercent = this.percentage + '%';
            this.$bar.width(showPercent).text(text);
        }
    ,
        start: function () {
            this.setPercentage(0);
            var that = this;
            this.$element.slideDown();
            setTimeout(function () {
                that.move();
            }, 400);
        }
    ,
        goOn: function () {
            this.move();
        }
    ,
        pause: function () {
            clearTimeout(this.timeOut);
        }
    ,
        complete: function () {
            this.setPercentage(100);
            var that = this;
            setTimeout(function () {
                that.$element.slideUp();
            }, 800);
        }
    }


    /* ProgressBar PLUGIN DEFINITION
    * ========================== */

    var old = $.fn.progressBar

    $.fn.progressBar = function (option) {

        return this.each(function () {
            var $this = $(this)
              , data = $this.data('progressBar')
              , options = $.extend({}, $.fn.progressBar.defaults, typeof option == 'object' && option)
            if (!data) $this.data('progressBar', (data = new ProgressBar(this, options)));

            if (typeof option == 'number') data.setPercentage(option);
            else if (typeof option == 'string') data[option]();
            else if (options.percent && options.text) data.setText(options.percent, options.text)
        })
    }

    $.fn.progressBar.defaults = { step: 1, maxPercent: 95, interval: 100 };

    $.fn.progressBar.Constructor = ProgressBar;


    /* ProgressBar NO CONFLICT
    * =================== */

    $.fn.progressBar.noConflict = function () {
        $.fn.progressBar = old
        return this
    }

} (window.jQuery);


