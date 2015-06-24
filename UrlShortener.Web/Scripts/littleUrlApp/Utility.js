littleUrlApp.value('utility', (function() {
    function bootstrapAlerter(area) {
        var $alertArea = area;

        function alert(type, message) {
            $alertArea.empty();
            $alertArea.append(getBootstrapAlertHtml(type, message));
            $alertArea.children('.alert').addClass('in');
        }

        function getBootstrapAlertHtml(type, message) {
            var $element = $('<div class="alert alert-' + type + ' alert-dismissible fade" role="alert">\
  <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>\
</div>');
            $element.append(message);
            return $element;
        }

        return {
            danger: function(message) {
                alert('danger', message);
            },
            success: function(message) {
                alert('success', message);
            }
        };
    }

    function htmlEncode(value) {
        return $('<div/>').text(value).html();
    }

    return {
        bootstrapAlerter: bootstrapAlerter,
        htmlEncode: htmlEncode
    }
})());