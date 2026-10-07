/**
 * EMPLOYEE EDUCATION DETAILS PORTAL - CLIENT JAVASCRIPT
 * 100% Offline Compatible Searchable Dropdown & UI Enhancements
 */

(function ($) {
    "use strict";

    window.EduApp = {
        /**
         * Converts any standard HTML <select> into a modern Searchable Dropdown with live search filter
         */
        makeSearchable: function (selector, placeholderText) {
            var $selects = $(selector);

            $selects.each(function () {
                var $select = $(this);
                if ($select.data('searchable-initialized')) {
                    EduApp.refreshSearchable($select);
                    return;
                }

                $select.data('searchable-initialized', true);
                $select.hide();

                var placeholder = placeholderText || $select.find('option:first').text() || '-- Select Option --';

                // Wrap and UI components
                var $wrap = $('<div class="searchable-select-wrap"></div>');
                var $trigger = $('<button type="button" class="searchable-select-trigger" tabindex="0"></button>');
                var $triggerText = $('<span class="trigger-text"></span>');
                var $triggerArrow = $('<i class="fas fa-chevron-down trigger-arrow"></i>');
                $trigger.append($triggerText).append($triggerArrow);

                var $dropdown = $('<div class="searchable-select-dropdown"></div>');
                var $searchBox = $('<div class="searchable-select-search-box"></div>');
                var $searchIcon = $('<i class="fas fa-search searchable-select-search-icon"></i>');
                var $searchInput = $('<input type="text" class="searchable-select-search-input" placeholder="Type here to search..." autocomplete="off" />');
                var $clearBtn = $('<span class="searchable-select-clear-btn" title="Clear search" style="display:none;">&times;</span>');
                $searchBox.append($searchIcon).append($searchInput).append($clearBtn);

                var $optionsList = $('<ul class="searchable-select-options"></ul>');
                var $noResults = $('<div class="searchable-select-no-results" style="display:none;"><i class="fas fa-info-circle mr-1"></i> No matching options found</div>');

                $dropdown.append($searchBox).append($optionsList).append($noResults);
                $wrap.append($trigger).append($dropdown);
                $select.after($wrap);

                // Store references
                $select.data('searchable-wrap', $wrap);
                $select.data('searchable-trigger', $triggerText);
                $select.data('searchable-options', $optionsList);
                $select.data('searchable-search', $searchInput);
                $select.data('searchable-clear', $clearBtn);
                $select.data('searchable-placeholder', placeholder);

                // Populate options
                EduApp.buildSearchableOptions($select);

                // Check disabled state
                if ($select.is(':disabled')) {
                    $trigger.prop('disabled', true).addClass('disabled');
                }

                function openDropdown() {
                    if ($select.is(':disabled')) return;
                    $('.searchable-select-wrap').not($wrap).removeClass('open drop-up');

                    // Drop-up calculation if near bottom
                    var winHeight = $(window).height();
                    var triggerOffset = $trigger.offset();
                    if (triggerOffset) {
                        var spaceBelow = winHeight - (triggerOffset.top - $(window).scrollTop() + $trigger.outerHeight());
                        if (spaceBelow < 280 && triggerOffset.top > 280) {
                            $wrap.addClass('drop-up');
                        } else {
                            $wrap.removeClass('drop-up');
                        }
                    }

                    $wrap.addClass('open');
                    $searchInput.val('');
                    $clearBtn.hide();
                    filterOptions('');
                    setTimeout(function () {
                        $searchInput.focus();
                    }, 50);
                }

                function closeDropdown() {
                    $wrap.removeClass('open drop-up');
                }

                // Events
                $trigger.on('click', function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    if ($wrap.hasClass('open')) {
                        closeDropdown();
                    } else {
                        openDropdown();
                    }
                });

                // Keypress on trigger directly opens and starts typing
                $trigger.on('keydown', function (e) {
                    if (e.key === 'ArrowDown' || e.key === 'Enter' || e.key === ' ') {
                        e.preventDefault();
                        openDropdown();
                    } else if (e.key.length === 1 && !e.ctrlKey && !e.altKey && !e.metaKey) {
                        openDropdown();
                        $searchInput.val(e.key);
                        $clearBtn.show();
                        filterOptions(e.key);
                    }
                });

                $clearBtn.on('click', function (e) {
                    e.stopPropagation();
                    $searchInput.val('').focus();
                    $clearBtn.hide();
                    filterOptions('');
                });

                function filterOptions(query) {
                    var terms = query.toLowerCase().trim().split(/\s+/).filter(Boolean);
                    var $items = $optionsList.find('.searchable-select-option');
                    var matchCount = 0;

                    $items.each(function () {
                        var rawText = $(this).attr('data-text') || $(this).text();
                        var lowerText = rawText.toLowerCase();
                        var isMatch = true;

                        if (terms.length > 0) {
                            for (var i = 0; i < terms.length; i++) {
                                if (lowerText.indexOf(terms[i]) === -1) {
                                    isMatch = false;
                                    break;
                                }
                            }
                        }

                        if (isMatch) {
                            $(this).removeClass('hidden');
                            matchCount++;
                            if (terms.length > 0) {
                                var highlighted = EduApp.highlightText(rawText, terms);
                                $(this).find('.option-label').html(highlighted);
                            } else {
                                $(this).find('.option-label').text(rawText);
                            }
                        } else {
                            $(this).addClass('hidden');
                        }
                    });

                    $items.removeClass('highlighted');
                    if (query !== '') {
                        $items.filter(':not(.hidden):first').addClass('highlighted');
                    }

                    if (matchCount === 0) {
                        $noResults.show();
                    } else {
                        $noResults.hide();
                    }
                }

                // Live Search Input Filter
                $searchInput.on('input keyup', function (e) {
                    if (e.key === 'ArrowDown' || e.key === 'ArrowUp' || e.key === 'Enter') {
                        return; // handled by keydown
                    }
                    var q = $(this).val();
                    if (q) {
                        $clearBtn.show();
                    } else {
                        $clearBtn.hide();
                    }
                    filterOptions(q);
                });

                // Keyboard handling inside search input
                $searchInput.on('keydown', function (e) {
                    var $visible = $optionsList.find('.searchable-select-option:not(.hidden)');
                    var $highlighted = $visible.filter('.highlighted');

                    if (e.key === 'ArrowDown') {
                        e.preventDefault();
                        if ($visible.length === 0) return;
                        var nextIndex = 0;
                        if ($highlighted.length > 0) {
                            var currIndex = $visible.index($highlighted);
                            nextIndex = (currIndex + 1) % $visible.length;
                        }
                        $visible.removeClass('highlighted');
                        var $next = $visible.eq(nextIndex).addClass('highlighted');
                        EduApp.scrollOptionIntoView($optionsList, $next);
                    } else if (e.key === 'ArrowUp') {
                        e.preventDefault();
                        if ($visible.length === 0) return;
                        var prevIndex = $visible.length - 1;
                        if ($highlighted.length > 0) {
                            var currIndex = $visible.index($highlighted);
                            prevIndex = (currIndex - 1 + $visible.length) % $visible.length;
                        }
                        $visible.removeClass('highlighted');
                        var $prev = $visible.eq(prevIndex).addClass('highlighted');
                        EduApp.scrollOptionIntoView($optionsList, $prev);
                    } else if (e.key === 'Enter') {
                        e.preventDefault();
                        if ($highlighted.length > 0) {
                            $highlighted.trigger('click');
                        } else if ($visible.length > 0) {
                            $visible.first().trigger('click');
                        }
                    } else if (e.key === 'Escape' || e.key === 'Tab') {
                        closeDropdown();
                        $trigger.focus();
                    }
                });

                // Sync on external select change
                $select.on('change', function () {
                    EduApp.updateTriggerText($select);
                });
            });
        },

        highlightText: function (text, terms) {
            if (!terms || terms.length === 0) return $('<div>').text(text).html();
            var escapedTerms = terms.map(function (t) {
                return t.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
            }).filter(Boolean);
            if (escapedTerms.length === 0) return $('<div>').text(text).html();
            var regex = new RegExp('(' + escapedTerms.join('|') + ')', 'gi');
            var safeText = $('<div>').text(text).html();
            return safeText.replace(regex, '<mark class="search-match">$1</mark>');
        },

        scrollOptionIntoView: function ($container, $item) {
            if (!$item || $item.length === 0) return;
            var containerTop = $container.scrollTop();
            var containerHeight = $container.innerHeight();
            var elemTop = $item.position().top + containerTop;
            var elemBottom = elemTop + $item.outerHeight();

            if (elemTop < containerTop) {
                $container.scrollTop(elemTop);
            } else if (elemBottom > (containerTop + containerHeight)) {
                $container.scrollTop(elemBottom - containerHeight);
            }
        },

        buildSearchableOptions: function ($select) {
            var $optionsList = $select.data('searchable-options');
            var $wrap = $select.data('searchable-wrap');
            var $trigger = $wrap ? $wrap.find('.searchable-select-trigger') : null;
            if (!$optionsList) return;

            $optionsList.empty();
            var currentVal = $select.val();

            $select.find('option').each(function () {
                var val = $(this).val();
                var text = $(this).text();
                var isSelected = (val === currentVal);

                var $li = $('<li class="searchable-select-option"></li>');
                $li.attr('data-value', val);
                $li.attr('data-text', text);

                var $textSpan = $('<span class="option-label"></span>').text(text);
                $li.append($textSpan);

                if (isSelected && val !== '') {
                    $li.addClass('selected');
                    $li.append('<i class="fas fa-check option-check text-primary"></i>');
                }

                $li.on('click', function (e) {
                    e.stopPropagation();
                    $select.val(val).trigger('change');
                    EduApp.updateTriggerText($select);
                    $wrap.removeClass('open drop-up');
                    if ($trigger) $trigger.focus();
                });

                $optionsList.append($li);
            });

            EduApp.updateTriggerText($select);
        },

        refreshSearchable: function ($select) {
            EduApp.buildSearchableOptions($select);
        },

        updateTriggerText: function ($select) {
            var $triggerText = $select.data('searchable-trigger');
            var placeholder = $select.data('searchable-placeholder') || '-- Select Option --';
            var $selectedOpt = $select.find('option:selected');

            if ($triggerText) {
                if ($selectedOpt.length > 0 && $selectedOpt.val() !== '') {
                    $triggerText.removeClass('trigger-placeholder').text($selectedOpt.text());
                } else {
                    $triggerText.addClass('trigger-placeholder').text(placeholder);
                }
            }

            // Update highlighted option in list
            var $optionsList = $select.data('searchable-options');
            if ($optionsList) {
                var currentVal = $select.val();
                $optionsList.find('.searchable-select-option').removeClass('selected').each(function () {
                    if ($(this).attr('data-value') === currentVal && currentVal !== '') {
                        $(this).addClass('selected');
                    }
                });
            }
        },

        /**
         * Dynamic cascading between Main Qualification and Sub Qualification
         */
        initCascadingDropdowns: function (mainDropdownId, subDropdownId) {
            var $main = $('#' + mainDropdownId);
            var $sub = $('#' + subDropdownId);

            if ($main.length === 0 || $sub.length === 0) return;

            // Cache original sub qualification options
            if (!$sub.data('all-options')) {
                var options = [];
                $sub.find('option').each(function () {
                    options.push({
                        value: $(this).val(),
                        text: $(this).text(),
                        mainCode: $(this).attr('data-main') || ''
                    });
                });
                $sub.data('all-options', options);
            }

            function filterSubOptions() {
                var selectedMain = $main.val();
                var currentSubVal = $sub.val();
                var allOptions = $sub.data('all-options');

                $sub.empty();
                $sub.append($('<option>', { value: '', text: '-- Select Sub Qualification / Stream --' }));

                if (!selectedMain) {
                    $.each(allOptions, function (i, opt) {
                        if (opt.value) {
                            $sub.append($('<option>', { value: opt.value, text: opt.text }));
                        }
                    });
                } else {
                    var matchedCount = 0;
                    $.each(allOptions, function (i, opt) {
                        if (!opt.value) return;
                        if (opt.mainCode === selectedMain || opt.mainCode === '' || opt.value === 'OTHER' || opt.value === 'GENERAL' || opt.value === 'NA') {
                            $sub.append($('<option>', { value: opt.value, text: opt.text }));
                            matchedCount++;
                        }
                    });

                    if (matchedCount === 0) {
                        $sub.append($('<option>', { value: 'GENERAL', text: 'General' }));
                        $sub.append($('<option>', { value: 'OTHER', text: 'Other (Specify in Specialization)' }));
                    }
                }

                // Restore previous selection if present
                if (currentSubVal && $sub.find('option[value="' + currentSubVal + '"]').length > 0) {
                    $sub.val(currentSubVal);
                }

                // Refresh searchable dropdown for sub qualification
                if ($sub.data('searchable-initialized')) {
                    EduApp.refreshSearchable($sub);
                }
            }

            $main.on('change', function () {
                filterSubOptions();
            });

            // Make both searchable
            EduApp.makeSearchable($main, '-- Search Main Qualification --');
            EduApp.makeSearchable($sub, '-- Search Sub Qualification / Stream --');

            // If main qualification is already selected on load (e.g. edit mode), filter immediately
            if ($main.val()) {
                filterSubOptions();
            }
        },

        initAllSearchable: function () {
            $('select.searchable-select, select[data-searchable="true"]').each(function () {
                EduApp.makeSearchable($(this));
            });
        },

        confirmSubmission: function () {
            return true;
        },

        confirmRevert: function (employeePcno) {
            return confirm("REVERT TO DRAFT:\n\nThis will unlock education records for PCNO: " + employeePcno + ", allowing the employee to edit or update their details.\n\nProceed with unlocking?");
        },

        filterTable: function (inputId, tableId) {
            $('#' + inputId).on('keyup', function () {
                var value = $(this).val().toLowerCase();
                $('#' + tableId + ' tbody tr').filter(function () {
                    $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
                });
            });
        },

        /**
         * Displays a sleek floating Toast Notification that automatically dismisses after durationMs (default 3000ms / 3 seconds)
         */
        showToast: function (message, type, durationMs) {
            type = type || 'info';
            durationMs = durationMs || 3000;

            var $container = $('#toastContainer');
            if (!$container.length) {
                $container = $('<div id="toastContainer" class="toast-container-custom"></div>');
                $('body').append($container);
            }

            var iconClass = 'fa-info-circle';
            var iconWrapClass = 'toast-icon-info';
            var progressClass = 'toast-progress-info';

            if (type === 'success') {
                iconClass = 'fa-check-circle';
                iconWrapClass = 'toast-icon-success';
                progressClass = 'toast-progress-success';
            } else if (type === 'warning') {
                iconClass = 'fa-exclamation-circle';
                iconWrapClass = 'toast-icon-warning';
                progressClass = 'toast-progress-warning';
            } else if (type === 'danger') {
                iconClass = 'fa-exclamation-triangle';
                iconWrapClass = 'toast-icon-danger';
                progressClass = 'toast-progress-danger';
            }

            var $toast = $('<div class="toast-custom toast-' + type + '">' +
                '<div class="toast-icon ' + iconWrapClass + '"><i class="fas ' + iconClass + '"></i></div>' +
                '<div class="toast-content">' +
                    '<div class="toast-message">' + message + '</div>' +
                '</div>' +
                '<button type="button" class="toast-close-btn" aria-label="Close" title="Dismiss">&times;</button>' +
                '<div class="toast-progress ' + progressClass + '" style="animation-duration: ' + durationMs + 'ms;"></div>' +
            '</div>');

            $container.append($toast);

            setTimeout(function () {
                $toast.addClass('show');
            }, 20);

            var isDismissed = false;
            function dismissToast() {
                if (isDismissed) return;
                isDismissed = true;
                $toast.removeClass('show').addClass('hide');
                setTimeout(function () {
                    $toast.remove();
                }, 350);
            }

            $toast.find('.toast-close-btn').on('click', function () {
                dismissToast();
            });

            var timer = setTimeout(function () {
                dismissToast();
            }, durationMs);

            $toast.on('mouseenter', function () {
                clearTimeout(timer);
                $toast.find('.toast-progress').css('animation-play-state', 'paused');
            }).on('mouseleave', function () {
                $toast.find('.toast-progress').css('animation-play-state', 'running');
                timer = setTimeout(function () {
                    dismissToast();
                }, 1500);
            });
        },

        /**
         * Displays the Caution Confirmation Modal before submitting records
         */
        showSubmitCaution: function (triggerElement, customMsg) {
            EduApp._pendingSubmitTrigger = triggerElement;
            if (customMsg) {
                $('#submitCautionMessageText').text(customMsg);
            } else {
                $('#submitCautionMessageText').text("Are you sure you want to finalize and submit your education records? Once submitted, they will be locked as official entries and cannot be edited.");
            }
            $('#submitCautionModal').addClass('active');
        },

        hideSubmitCaution: function () {
            $('#submitCautionModal').removeClass('active');
            EduApp._pendingSubmitTrigger = null;
        }
    };

    // Caution Confirmation Modal Handlers
    $(document).on('click', '#btnCancelCautionSubmit', function (e) {
        e.preventDefault();
        e.stopPropagation();
        EduApp.hideSubmitCaution();
    });

    $(document).on('click', '#btnConfirmCautionSubmit', function (e) {
        e.preventDefault();
        e.stopPropagation();
        var trigger = EduApp._pendingSubmitTrigger;
        EduApp.hideSubmitCaution();
        if (trigger) {
            var $t = $(trigger);
            var btnName = $t.attr('name');
            var btnVal = $t.val() || '';
            var form = $t.closest('form')[0];

            if (form) {
                if (btnName && !$('input[name="' + btnName + '"][type="hidden"]').length) {
                    var $hidden = $('<input type="hidden">').attr('name', btnName).val(btnVal);
                    $(form).append($hidden);
                }
                if (typeof form.submit === 'function') {
                    form.submit();
                } else {
                    HTMLFormElement.prototype.submit.call(form);
                }
            } else {
                trigger.__confirmed = true;
                trigger.click();
            }
        }
    });

    $(document).on('click', '#submitCautionModal', function (e) {
        if (e.target === this) {
            EduApp.hideSubmitCaution();
        }
    });

    $(document).on('keydown', function (e) {
        if (e.key === 'Escape' && $('#submitCautionModal').hasClass('active')) {
            EduApp.hideSubmitCaution();
        }
    });

    // Intercept submit buttons to show caution message before submitting
    $(document).on('click', '.btn-submit-with-caution', function (e) {
        if (this.__confirmed) {
            this.__confirmed = false;
            return true;
        }

        // Check if this is the modal's Save & Submit button
        var isModalRecord = (this.id && this.id.indexOf('btnSaveAndSubmit') > -1);
        if (isModalRecord) {
            var mainVal = $('select[id$="ddlMainQual"]').val();
            var subVal = $('select[id$="ddlSubQual"]').val();
            var yrVal = $('select[id$="ddlYearPassed"]').val();
            var specVal = $.trim($('input[id$="txtSpecialization"]').val());
            var univVal = $('select[id$="ddlUniversity"]').val();
            var cgpaVal = $.trim($('input[id$="txtCgpaMarks"]').val());
            var gradeVal = $('select[id$="ddlGrade"]').val();
            // If required fields are blank, let native submit run so validation error shows in modal
            if (!mainVal || !subVal || !yrVal || !specVal || !univVal || !cgpaVal || !gradeVal) {
                return true;
            }
        }

        e.preventDefault();
        var msg = isModalRecord 
            ? "Are you sure you want to save and submit this qualification? Once submitted, it will be locked as an official entry and cannot be edited."
            : "Are you sure you want to finalize and submit your education records? Once submitted, they will be locked as official entries and cannot be edited.";
        EduApp.showSubmitCaution(this, msg);
        return false;
    });

    // Global click outside listener to close any open dropdowns
    $(document).on('click', function (e) {
        if (!$(e.target).closest('.searchable-select-wrap').length) {
            $('.searchable-select-wrap').removeClass('open drop-up');
        }
    });

    /**
     * Non-blocking intranet employee photo loader with strict 1500ms timeout.
     * Prevents browser hangs/freezes when operating outside the company intranet.
     */
    EduApp.initIntranetPhotos = function () {
        $('img[data-intranet-photo]').each(function () {
            var $img = $(this);
            var url = $img.attr('data-intranet-photo');
            if (!url) return;

            var probe = new Image();
            var timedOut = false;
            var timer = setTimeout(function () {
                timedOut = true;
                probe.src = '';
            }, 1500);

            probe.onload = function () {
                if (!timedOut) {
                    clearTimeout(timer);
                    $img.attr('src', url);
                }
            };
            probe.onerror = function () {
                if (!timedOut) {
                    clearTimeout(timer);
                }
            };
            probe.src = url;
        });
    };

    // Auto-initialize all searchable dropdowns and intranet photos on page ready
    $(document).ready(function () {
        EduApp.initAllSearchable();
        EduApp.initIntranetPhotos();
    });

    // Support for ASP.NET AJAX partial postbacks if present
    if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            EduApp.initAllSearchable();
            EduApp.initIntranetPhotos();
        });
    }

})(jQuery);
