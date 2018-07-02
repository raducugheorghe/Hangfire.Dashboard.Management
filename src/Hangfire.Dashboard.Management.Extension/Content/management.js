﻿(function (hangfire) {

    hangfire.Management = (function () {
        function Management() {
            this._initialize();
        }
        Management.prototype._initialize = function () {

            $('.js-management').each(function () {
                var container = this;
                
                $(this).on('click', '.js-management-input-commands',
                   function (e) {
                       var $this = $(this);
                       var confirmText = $this.data('confirm');

                       var id = $this.attr("input-id");
                       var send = {};
                       $("input[id^='" + id + "']", container).each(function() {
                           if ($(this).is('[type=checkbox]')) {
                            if ($(this).is(':checked')) {
                              send[$(this).attr('id')] = "on";
                            }
                           } else {
                            send[$(this).attr('id')] = $(this).val();
                           }
                       });

                       $("div[id^='" + id + "']", container).each(function () {
                           
                           send[$(this).attr('id')] = $(this).data('date');
                       });

                       if (!!$this.attr('schedule')) {
                           send[id + '_schedule'] = $this.attr("schedule");
                       }

                       if (!confirmText || confirm(confirmText)) {
                           $this.prop('disabled');
                           $this.button('loading');
                           
                           $.post($this.data('url'), send, function (data) {
                               Hangfire.Management.alertSuccess(id, "A Task has been created. <a href=\"" + data.jobLink + "\">View Job</a>");
                           }).fail(function (xhr, status, error) {
                               Hangfire.Management.alertError(id,"There was an error. " + error);
                           }).always(function() {
                               $this.removeProp('disabled');
                               $this.button('reset');
                           });
                       }

                       e.preventDefault();
                   });
            });
        };

        Management.alertSuccess = function(id, message) {
            $('#' + id + '_success')
                .html('<div class="alert alert-success"><a class="close" data-dismiss="alert">×</a><strong>Task Created! </strong><span>' +
                    message +
                    '</span></div>');
        }

        Management.alertError = function(id, message) {
            $('#' + id + '_error')
                .html('<div class="alert alert-danger"><a class="close" data-dismiss="alert">×</a><strong>Error! </strong><span>' +
                    message +
                    '</span></div>');
        }

        return Management;

    })();

})(window.Hangfire = window.Hangfire || {});

function loadManagement() {
    Hangfire.management = new Hangfire.Management();

    var link = document.createElement('link');
    link.setAttribute("rel", "stylesheet");
    link.setAttribute("type", "text/css");
    link.setAttribute("href", 'https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datetimepicker/3.1.4/css/bootstrap-datetimepicker.min.css');
    document.getElementsByTagName("head")[0].appendChild(link);

    var url = "https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datetimepicker/3.1.4/js/bootstrap-datetimepicker.min.js";
    $.getScript(url,
        function() {
            $(function() {
                $("div[id$='_datetimepicker']").each(function() {
                    $(this).datetimepicker();
                });

            });
        });
}

if (window.attachEvent) {
    window.attachEvent('onload', loadManagement);
} else {
    if (window.onload) {
        var curronload = window.onload;
        var newonload = function (evt) {
            curronload(evt);
            loadManagement(evt);
        };
        window.onload = newonload;
    } else {
        window.onload = loadManagement;
    }
}