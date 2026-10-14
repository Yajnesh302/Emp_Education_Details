/**
 * EMPLOYEE EDUCATION DETAILS PORTAL - CLIENT JAVASCRIPT
 * 100% Offline Compatible Searchable Dropdown & UI Enhancements
 */

(function ($) {
    "use strict";

    window.EduApp = {
        /**
         * Checks if a value is null, whitespace, or a placeholder symbol like '*', '-', '?', etc.
         */
        isPlaceholder: function (val) {
            if (!val) return true;
            var v = $.trim(val);
            if (v === '' || v === '*' || v === '**' || v === '***' || 
                v === '-' || v === '--' || v === '---' || 
                v === '.' || v === '..' || v === '?' || v === '??') return true;
            var u = v.toUpperCase();
            return (u === 'NOT SPECIFIED' || u === 'UNKNOWN' || u === 'NULL');
        },

        /**
         * Specialization validator: Allows 'N/A', 'NA', 'General', 'Not Applicable' etc.,
         * but blocks empty values and placeholder symbols ('*', '?', etc.).
         */
        isInvalidSpec: function (val) {
            if (!val) return true;
            var v = $.trim(val);
            if (v === '' || v === '*' || v === '**' || v === '***' || v === '?' || v === '??') return true;
            return false;
        },

        /**
         * Displays an instant validation error inside the modal without page reload
         */
        showModalError: function (msg) {
            var $clientBanner = $('#modalClientErrorBanner');
            var $clientText = $('#modalClientErrorText');
            if ($clientBanner.length) {
                $clientText.html(msg);
                $clientBanner.slideDown(150);
            }
            var $serverPnl = $('[id$="pnlFormError"]');
            if ($serverPnl.length && $serverPnl.is(':visible')) {
                $serverPnl.hide();
            }
            // Scroll modal body to top so error banner is immediately visible
            var $modalBody = $('[id$="pnlModal"] .modal-body');
            if ($modalBody.length) {
                $modalBody.scrollTop(0);
            }
        },

        /**
         * Clears all error banners inside the modal
         */
        clearModalError: function () {
            $('#modalClientErrorBanner').slideUp(100);
            $('[id$="pnlFormError"]').slideUp(100);
        },

        /**
         * Evaluates all modal inputs for missing or placeholder values.
         * Returns { msg: string, $focus: jQueryElement } or null if valid.
         */
        getModalValidationError: function () {
            var $main = $('select[id$="ddlMainQual"]');
            var $sub = $('select[id$="ddlSubQual"]');
            var $yr = $('select[id$="ddlYearPassed"]');
            var $spec = $('input[id$="txtSpecialization"]');
            var $univ = $('select[id$="ddlUniversity"]');
            var $coll = $('select[id$="ddlCollege"]');
            var $customColl = $('input[id$="txtCustomCollege"]');
            var $scoreType = $('select[id$="ddlScoreType"]');
            var $score = $('input[id$="txtCgpaMarks"]');
            var $grade = $('select[id$="ddlGrade"]');

            var mainVal = $main.val();
            if (!mainVal || EduApp.isPlaceholder(mainVal)) {
                return { msg: "Please select a valid Main Qualification from the list.", $focus: $main };
            }

            var subVal = $sub.val();
            if (!subVal || EduApp.isPlaceholder(subVal)) {
                return { msg: "Please select a valid Sub Qualification / Stream from the list.", $focus: $sub };
            }

            var yrVal = $yr.val();
            if (!yrVal || EduApp.isPlaceholder(yrVal)) {
                return { msg: "Please select a valid Passing Year from the list.", $focus: $yr };
            }

            var specVal = $.trim($spec.val());
            if (EduApp.isInvalidSpec(specVal)) {
                return { msg: "Please enter Specialization / Major Subject (or 'N/A' if not applicable). Placeholder values like '*' are not allowed.", $focus: $spec };
            }

            var isCert = EduApp.isCertificateQualification($main.val());
            var univVal = $univ.val();
            if (!univVal || EduApp.isPlaceholder(univVal)) {
                var fieldName = isCert ? "Certifying Body / Issuing Authority" : "University / Board";
                return { msg: "Please select a valid " + fieldName + " from the list. Placeholder values like '*' are not allowed.", $focus: $univ };
            }

            var collVal = $coll.val();
            if (!collVal || EduApp.isPlaceholder(collVal)) {
                if (isCert) {
                    collVal = 'Direct Certification / Same as Issuing Authority';
                    $coll.val(collVal);
                } else {
                    return { msg: "Please select a valid College / School Name from the list.", $focus: $coll };
                }
            }
            if (collVal === 'OTHER' || (collVal && collVal.indexOf('Other') === 0)) {
                var customVal = $.trim($customColl.val());
                if (!customVal || EduApp.isPlaceholder(customVal)) {
                    return { msg: "Please specify your College / School Name in the text box provided. Placeholder values like '*' are not allowed.", $focus: $customColl };
                }
            }

            var rawScore = $.trim($score.val());
            if (!rawScore || EduApp.isPlaceholder(rawScore)) {
                return { msg: "Please enter valid Marks / Percentage / CGPA. Placeholder values like '*' are not allowed.", $focus: $score };
            }

            var scoreType = $scoreType.val() || 'Percentage';
            var cleanNumStr = rawScore.replace(/%/g, '').replace(/cgpa/gi, '').trim();
            var scoreNum = parseFloat(cleanNumStr);
            if (!isNaN(scoreNum)) {
                if (scoreType === 'Percentage') {
                    if (scoreNum < 0 || scoreNum > 100) {
                        return { msg: "Percentage must be a valid value between 0.00% and 100.00%.", $focus: $score };
                    }
                } else if (scoreType === 'CGPA') {
                    if (scoreNum < 0 || scoreNum > 10) {
                        return { msg: "CGPA must be a valid value between 0.00 and 10.00 (on standard 10-point scale).", $focus: $score };
                    }
                }
            }

            var gradeVal = $grade.val();
            if (!gradeVal || EduApp.isPlaceholder(gradeVal)) {
                return { msg: "Please select a valid Grade / Division from the list.", $focus: $grade };
            }

            return null;
        },

        /**
         * Validates the modal form prior to save.
         * Returns false and displays inline error if invalid; returns true if valid.
         */
        validateModalForm: function (isFinalSubmit) {
            var valErr = EduApp.getModalValidationError();
            if (valErr) {
                EduApp.showModalError(valErr.msg);
                if (valErr.$focus && valErr.$focus.length) {
                    var $trig = valErr.$focus.data('searchable-trigger');
                    if ($trig && $trig.length) $trig.focus();
                    else valErr.$focus.focus();
                }
                return false;
            }

            EduApp.clearModalError();
            // Sync hidden fields before postback
            $('input[id$="hfSelectedMainQual"]').val($('select[id$="ddlMainQual"]').val() || '');
            $('input[id$="hfSelectedSubQual"]').val($('select[id$="ddlSubQual"]').val() || '');
            $('input[id$="hfSelectedSubQualName"]').val($('select[id$="ddlSubQual"]').find('option:selected').text() || '');
            return true;
        },

        /**
         * Opens the Add Qualification modal seamlessly with 0-latency and zero page refresh
         */
        openAddModal: function () {
            var $modal = $('[id$="pnlModal"]');
            if ($modal.length === 0) return;

            // Reset record ID and hidden fields
            $('[id$="hfEditEduId"]').val('0');
            $('[id$="hfModalOpen"]').val('1');
            $('[id$="hfSelectedMainQual"]').val('');
            $('[id$="hfSelectedSubQual"]').val('');
            $('[id$="hfSelectedSubQualName"]').val('');

            // Reset modal title
            $('[id$="litModalTitle"], #lblModalTitleText').text('Add Qualification Record');

            // Reset Dropdowns
            var $main = $('[id$="ddlMainQual"]');
            var $sub = $('[id$="ddlSubQual"]');
            var $univ = $('[id$="ddlUniversity"]');
            var $college = $('[id$="ddlCollege"]');
            var $year = $('[id$="ddlYearPassed"]');
            var $grade = $('[id$="ddlGrade"]');

            if ($main.length) {
                $main.val('').trigger('change');
                EduApp.updateTriggerText($main);
            }
            if ($sub.length) {
                if (typeof $sub.data('filter-fn') === 'function') {
                    $sub.data('filter-fn')();
                } else {
                    $sub.empty().append($('<option>', { value: '', text: '-- Select Sub Qualification / Stream --' }));
                    $sub.val('').trigger('change');
                    EduApp.updateTriggerText($sub);
                }
            }
            if ($univ.length) {
                $univ.val('').trigger('change');
                EduApp.updateTriggerText($univ);
            }
            if ($college.length) {
                $college.val('').trigger('change');
                EduApp.updateTriggerText($college);
            }
            if ($year.length) {
                $year.val('').trigger('change');
                EduApp.updateTriggerText($year);
            }
            if ($grade.length) {
                $grade.val('').trigger('change');
                EduApp.updateTriggerText($grade);
            }

            // Reset inputs
            $('[id$="txtSpecialization"]').val('');
            $('[id$="txtCustomCollege"]').val('');
            $('[id$="pnlCustomCollege"]').hide();
            $('[id$="ddlScoreType"]').val('Percentage');
            $('[id$="txtCgpaMarks"]').val('').attr('placeholder', 'e.g. 78.50%');
            $('[id$="chkIsHighest"]').prop('checked', false);
            $('[id$="txtRemarks"]').val('');
            EduApp.clearModalError();
            EduApp.adaptFieldsForQualification('');

            // Display modal smoothly with flex positioning
            $modal.css('display', 'flex').hide().fadeIn(150);

            // Set focus to the main qualification selector
            setTimeout(function () {
                var $trig = $main.data('searchable-trigger');
                if ($trig && $trig.length) {
                    $trig.focus();
                } else if ($main.length) {
                    $main.focus();
                }
            }, 60);
        },

        /**
         * Closes the Qualification modal seamlessly without refreshing the page
         */
        closeModal: function () {
            var $modal = $('[id$="pnlModal"]');
            if ($modal.length) {
                $modal.fadeOut(120, function () {
                    $(this).css('display', 'none');
                });
            }
            $('input[id$="hfModalOpen"]').val('0');
            $('input[id$="hfEditEduId"]').val('0');
            EduApp.clearModalError();
        },

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

            function getAllSubOptions() {
                // Priority 1: Serialized global array from server (guaranteed to have all data-main values)
                if (window.EDU_ALL_SUB_QUALS && window.EDU_ALL_SUB_QUALS.length > 0) {
                    return window.EDU_ALL_SUB_QUALS.map(function (item) {
                        return {
                            value: item.code,
                            text: item.name,
                            mainCode: item.main || ''
                        };
                    });
                }

                // Priority 2: Cache from DOM
                if (!$sub.data('all-options') || $sub.data('all-options').length <= 2) {
                    var options = [];
                    $sub.find('option').each(function () {
                        options.push({
                            value: $(this).val(),
                            text: $(this).text(),
                            mainCode: $(this).attr('data-main') || ''
                        });
                    });
                    if (options.length > 2) {
                        $sub.data('all-options', options);
                    }
                }
                return $sub.data('all-options') || [];
            }

            function normalizeQualCode(code) {
                if (!code) return '';
                return code.toUpperCase().replace(/[^A-Z0-9]/g, '');
            }

            // Related degree families to ensure complete options for equivalent qualifications
            var degreeFamilies = {
                // Engineering UG
                'BE': ['BE', 'BTECH', 'BSCEN', 'AMIE', 'BME', 'DENG'],
                'BTECH': ['BTECH', 'BE', 'BSCEN', 'AMIE', 'BME', 'DENG'],
                'BSCEN': ['BSCEN', 'BE', 'BTECH', 'AMIE'],
                'AMIE': ['AMIE', 'BE', 'BTECH'],
                'BME': ['BME', 'BE', 'BTECH'],

                // Engineering PG
                'ME': ['ME', 'MTECH', 'MSENG', 'MENG', 'BE', 'BTECH'],
                'MTECH': ['MTECH', 'ME', 'MSENG', 'MENG', 'BE', 'BTECH'],
                'MSENG': ['MSENG', 'ME', 'MTECH', 'BE', 'BTECH'],
                'MENG': ['MENG', 'ME', 'MTECH', 'BE', 'BTECH'],

                // Diploma
                'DENG': ['DENG', 'DIPL', 'DIP', 'BE', 'BTECH'],
                'DIPL': ['DIPL', 'DENG', 'DIP'],

                // Higher Secondary (PUC / 12th)
                'PUC': ['PUC', 'INTER', 'SCI', '12TH'],
                'INTER': ['INTER', 'PUC', 'SCI', '12TH'],
                'SCI': ['SCI', '12TH', 'PUC', 'INTER'],
                '12TH': ['12TH', 'SCI', 'PUC', 'INTER'],

                // Secondary (10th)
                'MTRI': ['MTRI', '10TH', 'BMTRI'],
                '10TH': ['10TH', 'MTRI', 'BMTRI'],
                'BMTRI': ['BMTRI', 'MTRI', '10TH'],

                // Business & Management
                'BBA': ['BBA', 'BBM', 'BBS'],
                'BBM': ['BBM', 'BBA', 'BBS'],
                'BBS': ['BBS', 'BBA', 'BBM'],
                'MBA': ['MBA', 'MMS', 'MBM'],
                'MMS': ['MMS', 'MBA', 'MBM'],
                'MBM': ['MBM', 'MBA', 'MMS'],

                // Computer Applications
                'BCA': ['BCA', 'MCA'],
                'MCA': ['MCA', 'BCA'],

                // Science
                'BSC': ['BSC', 'MSC'],
                'MSC': ['MSC', 'BSC'],

                // Commerce
                'BCOM': ['BCOM', 'MCOM'],
                'MCOM': ['MCOM', 'BCOM'],

                // Arts
                'BA': ['BA', 'MA'],
                'MA': ['MA', 'BA'],

                // Law
                'LLB': ['LLB', 'BAL', 'LLM'],
                'BAL': ['BAL', 'LLB', 'LLM'],
                'LLM': ['LLM', 'LLB'],

                // Medicine
                'MBBS': ['MBBS', 'MD', 'MS', 'BDS', 'BVSC'],
                'MD': ['MD', 'MBBS', 'MS'],
                'MS': ['MS', 'MBBS', 'MD'],
                'BDS': ['BDS', 'MBBS'],

                // Vocational / Apprenticeship / ITI / Certificate (LRDE Trades)
                'ITI': ['ITI', 'NAC', 'NTC', 'NCVT', 'CC'],
                'NAC': ['NAC', 'ITI', 'NTC', 'NCVT', 'CC'],
                'NTC': ['NTC', 'NAC', 'ITI', 'NCVT', 'CC'],
                'NCVT': ['NCVT', 'NAC', 'NTC', 'ITI', 'CC'],
                'CC': ['CC', 'NAC', 'ITI', 'NTC', 'NCVT']
            };

            function filterSubOptions() {
                var rawSelected = ($main.val() || '').trim();
                var selectedNorm = normalizeQualCode(rawSelected);
                var currentSubVal = $sub.val() || $('input[id$="hfSelectedSubQual"]').val();
                var allOptions = getAllSubOptions();

                $sub.empty();
                $sub.append($('<option>', { value: '', text: '-- Select Sub Qualification / Stream --' }));

                if (!rawSelected) {
                    var seenAll = {};
                    $.each(allOptions, function (i, opt) {
                        if (opt.value && !seenAll[opt.value]) {
                            seenAll[opt.value] = true;
                            $sub.append($('<option>', { value: opt.value, text: opt.text }));
                        }
                    });
                } else {
                    var familyList = degreeFamilies[selectedNorm] || [selectedNorm];
                    var allowedMains = familyList.concat([rawSelected, selectedNorm]);
                    var seenCodes = {};
                    var regularOptions = [];
                    var fallbackOptions = [];

                    $.each(allOptions, function (i, opt) {
                        if (!opt.value) return;

                        var isUniversal = (opt.value === 'OTHER' || opt.value === 'GENERAL');
                        var optNorm = normalizeQualCode(opt.mainCode);
                        var isDirectMatch = (opt.mainCode === rawSelected || (optNorm && optNorm === selectedNorm));
                        var isFamilyMatch = (optNorm && allowedMains.indexOf(optNorm) !== -1);

                        if (isDirectMatch || isFamilyMatch) {
                            if (!seenCodes[opt.value]) {
                                seenCodes[opt.value] = true;
                                regularOptions.push(opt);
                            }
                        } else if (isUniversal) {
                            if (!seenCodes[opt.value]) {
                                seenCodes[opt.value] = true;
                                fallbackOptions.push(opt);
                            }
                        }
                    });

                    // Sort regular streams alphabetically
                    regularOptions.sort(function (a, b) {
                        return a.text.localeCompare(b.text);
                    });

                    // Append regular options
                    $.each(regularOptions, function (i, opt) {
                        $sub.append($('<option>', { value: opt.value, text: opt.text }));
                    });

                    // Append fallbacks at the end
                    if (fallbackOptions.length > 0) {
                        $.each(fallbackOptions, function (i, opt) {
                            $sub.append($('<option>', { value: opt.value, text: opt.text }));
                        });
                    } else if (regularOptions.length === 0) {
                        $sub.append($('<option>', { value: 'GENERAL', text: 'General' }));
                        $sub.append($('<option>', { value: 'OTHER', text: 'Other (Specify in Specialization)' }));
                    }
                }

                // Restore previous selection if present
                if (currentSubVal && $sub.find('option[value="' + currentSubVal + '"]').length > 0) {
                    $sub.val(currentSubVal);
                } else if (currentSubVal) {
                    // Custom stream or not in family - append to guarantee it is NEVER dropped
                    $sub.append($('<option>', { value: currentSubVal, text: currentSubVal }));
                    $sub.val(currentSubVal);
                }

                // Sync to hidden fields
                $('input[id$="hfSelectedSubQual"]').val($sub.val() || '');
                $('input[id$="hfSelectedSubQualName"]').val($sub.find('option:selected').text() || '');

                // Refresh searchable dropdown for sub qualification
                if ($sub.data('searchable-initialized')) {
                    EduApp.refreshSearchable($sub);
                }
            }

            // Store filter function reference on element
            $sub.data('filter-fn', filterSubOptions);

            $main.off('change.cascade').on('change.cascade', function () {
                var mVal = $(this).val();
                $('input[id$="hfSelectedMainQual"]').val(mVal);
                EduApp.clearModalError();
                filterSubOptions();
                EduApp.adaptFieldsForQualification(mVal);
            });

            $sub.off('change.cascade').on('change.cascade', function () {
                var sVal = $(this).val();
                $('input[id$="hfSelectedSubQual"]').val(sVal);
                $('input[id$="hfSelectedSubQualName"]').val($(this).find('option:selected').text() || '');
                EduApp.clearModalError();
            });

            // Make both searchable
            EduApp.makeSearchable($main, '-- Search Main Qualification --');
            EduApp.makeSearchable($sub, '-- Search Sub Qualification / Stream --');

            // If main qualification is selected in hidden field but not in dropdown, sync it
            var hfMainVal = $('input[id$="hfSelectedMainQual"]').val();
            if (!$main.val() && hfMainVal) {
                $main.val(hfMainVal);
                EduApp.updateTriggerText($main);
            }

            // If main qualification is already selected on load (e.g. edit mode), filter immediately
            if ($main.val()) {
                filterSubOptions();
                EduApp.adaptFieldsForQualification($main.val());
            }
        },

        /**
         * Determines if a qualification code or name represents a certificate, vocational, or apprenticeship qualification
         */
        isCertificateQualification: function (qualVal) {
            if (!qualVal) return false;
            var q = String(qualVal).trim().toUpperCase();
            var certCodes = ['CC', 'NAC', 'NTC', 'NCVT', 'ITI', 'ATC', 'CERT', 'CERTIFICATE', 'VOCATIONAL', 'APPRENTICE', 'TRAINING'];
            for (var i = 0; i < certCodes.length; i++) {
                if (q === certCodes[i] || q.indexOf(certCodes[i] + ' -') === 0 || q.indexOf('- ' + certCodes[i]) !== -1 || q.indexOf(certCodes[i]) !== -1) {
                    return true;
                }
            }
            return false;
        },

        /**
         * Dynamically adjusts form labels, help hints, and college defaults when a Certificate is selected
         */
        adaptFieldsForQualification: function (qualVal) {
            var isCert = EduApp.isCertificateQualification(qualVal);

            var $univLabel = $('#lblUniversity, #lblAdminUniversity, label[for*="ddlUniversity"]');
            var $helpCert = $('#helpUniversityCert, #helpAdminUniversityCert');
            var $sameCheckbox = $('#chkSameAsUniv, #chkAdminSameAsUniv');
            var $sameLabel = $('#lblSameAsUniv, #lblAdminSameAsUniv');
            var $collSelect = $('select[id$="ddlCollege"]');

            if (isCert) {
                // Adapt University field to Certifying Body / Issuing Authority
                if ($univLabel.length) {
                    $univLabel.html('<i class="fas fa-award text-primary mr-1"></i> Certifying Body / Issuing Authority / Institute <span class="req">*</span>');
                }
                if ($helpCert.length) {
                    $helpCert.slideDown(150);
                }

                // Adapt College checkbox label
                if ($sameLabel.length) {
                    $sameLabel.html('<i class="fas fa-certificate mr-1 text-primary"></i> Direct Certification / Issued directly by Organization (No Separate College)');
                }

                // If college is empty, not selected, or set to university campus, auto-select Direct Certification
                var currentColl = $collSelect.val();
                var directVal = 'Direct Certification / Same as Issuing Authority';
                if (!currentColl || currentColl === 'University Campus / Department' || currentColl === directVal) {
                    if ($collSelect.find('option[value="' + directVal + '"]').length === 0) {
                        $collSelect.append($('<option>', { value: directVal, text: directVal + ' (No Separate College)' }));
                    }
                    $collSelect.val(directVal);
                    if ($collSelect.data('searchable-initialized')) {
                        EduApp.updateTriggerText($collSelect);
                    }
                    $sameCheckbox.prop('checked', true);
                    $('[id$="pnlCustomCollege"]').hide();
                }
            } else {
                // Revert to standard University / Board labels
                if ($univLabel.length) {
                    $univLabel.html('University / Board / Recognized Institute <span class="req">*</span>');
                }
                if ($helpCert.length) {
                    $helpCert.slideUp(150);
                }

                if ($sameLabel.length) {
                    $sameLabel.html('<i class="fas fa-university mr-1 text-primary"></i> Studied directly at University Campus / Department (Same as University)');
                }

                // If currently set to Direct Certification, clear it
                var currentColl = $collSelect.val();
                if (currentColl === 'Direct Certification / Same as Issuing Authority') {
                    $collSelect.val('');
                    if ($collSelect.data('searchable-initialized')) {
                        EduApp.updateTriggerText($collSelect);
                    }
                    $sameCheckbox.prop('checked', false);
                }
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

        confirmRevert: function (employeePcno, onProceed) {
            EduApp.showConfirmDialog({
                title: "Revert Records to Draft",
                message: "This will unlock education records for PCNO: <strong>" + employeePcno + "</strong>, allowing the employee to edit or update their details.",
                type: "warning",
                confirmBtnText: "Yes, Revert to Draft",
                onConfirm: onProceed
            });
            return false;
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
        },

        /**
         * Universal Modern Confirmation Dialog (Replaces native browser confirm)
         */
        showConfirmDialog: function (options) {
            var opts = options || {};
            var title = opts.title || "Confirmation Required";
            var msg = opts.message || "Are you sure you want to proceed with this action?";
            var type = opts.type || "warning"; // 'danger' | 'warning' | 'success' | 'info'
            var confirmText = opts.confirmBtnText || "Yes, Confirm";
            var cancelText = opts.cancelBtnText || "Cancel";

            var $modal = $('#appConfirmModal');
            if ($modal.length === 0) {
                if (window.confirm(msg)) {
                    if (typeof opts.onConfirm === 'function') opts.onConfirm();
                }
                return;
            }

            $('#appConfirmTitle').text(title);
            $('#appConfirmMessageText').html(msg);

            var $callout = $('#appConfirmCallout');
            if (opts.callout) {
                $('#appConfirmCalloutText').html(opts.callout);
                $callout.show();
            } else {
                $callout.hide();
            }

            var $iconWrap = $('#appConfirmIconWrap');
            var $icon = $('#appConfirmIcon');
            var $btnOk = $('#btnAppConfirmOk');
            var $btnCancel = $('#btnAppConfirmCancel');

            $btnCancel.show().html('<i class="fas fa-times mr-1"></i> ' + cancelText);
            $btnOk.html('<i class="fas fa-check mr-1"></i> ' + confirmText);

            $btnOk.removeClass('btn-danger btn-warning btn-success btn-primary btn-edu-success text-dark');

            if (type === 'danger') {
                $iconWrap.css({ 'background': '#fee2e2', 'box-shadow': '0 0 0 8px rgba(254, 226, 226, 0.65)' });
                $icon.attr('class', 'fas fa-trash-alt text-danger');
                $btnOk.addClass('btn-danger');
            } else if (type === 'success') {
                $iconWrap.css({ 'background': '#dcfce7', 'box-shadow': '0 0 0 8px rgba(220, 252, 231, 0.65)' });
                $icon.attr('class', 'fas fa-check-circle text-success');
                $btnOk.addClass('btn-edu-success');
            } else if (type === 'info') {
                $iconWrap.css({ 'background': '#e0f2fe', 'box-shadow': '0 0 0 8px rgba(224, 242, 254, 0.65)' });
                $icon.attr('class', 'fas fa-info-circle text-primary');
                $btnOk.addClass('btn-primary');
            } else {
                // warning
                $iconWrap.css({ 'background': '#fef3c7', 'box-shadow': '0 0 0 8px rgba(254, 243, 199, 0.65)' });
                $icon.attr('class', 'fas fa-exclamation-triangle text-warning');
                $btnOk.addClass('btn-warning text-dark font-weight-bold');
            }

            EduApp._pendingConfirmCallback = opts.onConfirm;
            $modal.addClass('active');
        },

        /**
         * Universal Modern Alert Dialog (Replaces native browser alert)
         */
        showAlertDialog: function (options) {
            var opts = options || {};
            var title = opts.title || "Attention";
            var msg = opts.message || "";
            var type = opts.type || "warning";
            var btnText = opts.btnText || "Understood";

            EduApp.showConfirmDialog({
                title: title,
                message: msg,
                type: type,
                confirmBtnText: btnText,
                onConfirm: opts.onClose
            });

            // Hide cancel button for pure notification/alert
            $('#btnAppConfirmCancel').hide();
        },

        hideConfirmDialog: function () {
            $('#appConfirmModal').removeClass('active');
            EduApp._pendingConfirmCallback = null;
        }
    };

    // Global Modal Confirmation Event Handlers
    $(document).on('click', '#btnAppConfirmCancel', function (e) {
        e.preventDefault();
        e.stopPropagation();
        EduApp.hideConfirmDialog();
    });

    $(document).on('click', '#btnAppConfirmOk', function (e) {
        e.preventDefault();
        e.stopPropagation();
        var callback = EduApp._pendingConfirmCallback;
        EduApp.hideConfirmDialog();
        if (typeof callback === 'function') {
            callback();
        }
    });

    $(document).on('click', '#appConfirmModal', function (e) {
        if (e.target === this) {
            EduApp.hideConfirmDialog();
        }
    });

    $(document).on('keydown', function (e) {
        if (e.key === 'Escape' && $('#appConfirmModal').hasClass('active')) {
            EduApp.hideConfirmDialog();
        }
    });

    // Universal interceptor for all data-confirm actions (Replaces inline confirm popups)
    $(document).on('click', '[data-confirm]', function (e) {
        if (this.__confirmed) {
            this.__confirmed = false;
            return true;
        }

        e.preventDefault();
        e.stopPropagation();

        var trigger = this;
        var msg = $(this).attr('data-confirm');
        var title = $(this).attr('data-confirm-title') || 'Confirm Action';
        var type = $(this).attr('data-confirm-type') || 'warning';
        var btnText = $(this).attr('data-confirm-btn') || 'Yes, Confirm';
        var callout = $(this).attr('data-confirm-callout') || '';

        EduApp.showConfirmDialog({
            title: title,
            message: msg,
            callout: callout,
            type: type,
            confirmBtnText: btnText,
            onConfirm: function () {
                trigger.__confirmed = true;
                if ($(trigger).is('a')) {
                    var href = $(trigger).attr('href');
                    if (href && href.indexOf('javascript:') === 0) {
                        var jsCode = href.substring(11);
                        try {
                            new Function(jsCode)();
                        } catch (err) {
                            eval(jsCode);
                        }
                        return;
                    }
                }
                if (typeof trigger.click === 'function') {
                    trigger.click();
                }
            }
        });

        return false;
    });

    // Modernize window.alert to seamlessly redirect through the custom alert dialog
    window.alert = function (message) {
        EduApp.showAlertDialog({
            title: "Notification",
            message: message,
            type: "info",
            btnText: "OK"
        });
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
            trigger.__confirmed = true;
            trigger.click();
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

    // Intercept submit buttons to validate or show caution message before submitting
    $(document).on('click', '.btn-submit-with-caution', function (e) {
        if (this.__confirmed) {
            this.__confirmed = false;
            return true;
        }

        // Check if this is the modal's Save & Submit button
        var isModalRecord = (this.id && this.id.indexOf('btnSaveAndSubmit') > -1);

        if (isModalRecord) {
            var valErr = EduApp.getModalValidationError();
            if (valErr) {
                e.preventDefault();
                e.stopPropagation();
                EduApp.showModalError(valErr.msg);
                if (valErr.$focus && valErr.$focus.length) {
                    var $trig = valErr.$focus.data('searchable-trigger');
                    if ($trig && $trig.length) $trig.focus();
                    else valErr.$focus.focus();
                }
                return false;
            }

            // Clear any error if valid
            EduApp.clearModalError();
            // Sync hidden fields
            $('input[id$="hfSelectedMainQual"]').val($('select[id$="ddlMainQual"]').val() || '');
            $('input[id$="hfSelectedSubQual"]').val($('select[id$="ddlSubQual"]').val() || '');
            $('input[id$="hfSelectedSubQualName"]').val($('select[id$="ddlSubQual"]').find('option:selected').text() || '');
        } else {
            // Final submission from banner or bottom button
            // If there are records flagged with Details Pending, inform user cleanly
            if ($('.badge-record-incomplete').length > 0) {
                e.preventDefault();
                EduApp.showAlertDialog({
                    title: "Verification Required",
                    message: "You have qualification records tagged with <strong>'Details Pending'</strong>.<br><br>Please click <strong>Edit</strong> on those entries to complete all required details before submitting.",
                    type: "warning",
                    btnText: "Understood, Review Records"
                });
                return false;
            }
        }

        e.preventDefault();
        var msg = isModalRecord 
            ? "Are you sure you want to save and submit this qualification? Once submitted, it will be locked as an official entry and cannot be edited."
            : "Are you sure you want to finalize and submit your education records? Once submitted, they will be locked as official entries and cannot be edited.";
        EduApp.showSubmitCaution(this, msg);
        return false;
    });

    // Safeguard for Save Draft button
    $(document).on('click', '.btn-save-draft', function (e) {
        if (!EduApp.validateModalForm(false)) {
            e.preventDefault();
            e.stopPropagation();
            return false;
        }
        return true;
    });

    // Auto-clear modal error when user interacts with inputs
    $(document).on('input change', '[id$="pnlModal"] input, [id$="pnlModal"] select', function () {
        EduApp.clearModalError();
    });

    // Global click outside listener to close any open dropdowns
    $(document).on('click', function (e) {
        if (!$(e.target).closest('.searchable-select-wrap').length) {
            $('.searchable-select-wrap').removeClass('open drop-up');
        }
    });

    /**
     * Non-blocking intranet employee photo loader with smart caching and stable fallback.
     * Prevents browser hangs/freezes outside intranet while guaranteeing photos never disappear.
     */
    EduApp.initIntranetPhotos = function () {
        window._empPhotoCache = window._empPhotoCache || {};

        $('img[data-intranet-photo]').each(function () {
            var $img = $(this);
            var url = $img.attr('data-intranet-photo');
            if (!url) return;

            // If image is already actively displaying this photo, do nothing
            if ($img.attr('src') === url) {
                return;
            }

            // Check in-memory or sessionStorage cache
            var cached = window._empPhotoCache[url];
            if (!cached && window.sessionStorage) {
                try {
                    cached = sessionStorage.getItem('emp_photo_status_' + url);
                } catch (e) {}
            }

            if (cached === 'ok') {
                $img.attr('src', url);
                return;
            } else if (cached === 'fail') {
                // Verified unreachable, keep default avatar
                return;
            }

            // Probe non-blockingly with generous 4-second network window
            var probe = new Image();
            var timer = setTimeout(function () {
                window._empPhotoCache[url] = 'fail';
            }, 4000);

            probe.onload = function () {
                clearTimeout(timer);
                window._empPhotoCache[url] = 'ok';
                if (window.sessionStorage) {
                    try { sessionStorage.setItem('emp_photo_status_' + url, 'ok'); } catch (e) {}
                }
                $img.attr('src', url);
            };

            probe.onerror = function () {
                clearTimeout(timer);
                window._empPhotoCache[url] = 'fail';
                if (window.sessionStorage) {
                    try { sessionStorage.setItem('emp_photo_status_' + url, 'fail'); } catch (e) {}
                }
            };

            probe.src = url;
        });
    };

    function setupPartialPostbackHandlers() {
        if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            if (!prm._eduAppEndRequestBound) {
                prm.add_endRequest(function () {
                    EduApp.initAllSearchable();
                    EduApp.initIntranetPhotos();

                    // If modal state is 0, ensure modal backdrop is strictly hidden
                    var modalState = $('input[id$="hfModalOpen"]').val();
                    if (modalState === '0') {
                        $('[id$="pnlModal"]').css('display', 'none');
                    }
                });
                prm._eduAppEndRequestBound = true;
            }
        }
    }

    // Auto-initialize all searchable dropdowns and intranet photos on page ready
    $(document).ready(function () {
        EduApp.initAllSearchable();
        EduApp.initIntranetPhotos();
        setupPartialPostbackHandlers();

        // Close modal on backdrop click
        $(document).on('click', '.custom-modal-backdrop', function (e) {
            if ($(e.target).hasClass('custom-modal-backdrop') || $(e.target).is('[id$="pnlModal"]')) {
                EduApp.closeModal();
            }
        });

        // Close modal on Escape key press
        $(document).on('keydown', function (e) {
            if (e.key === 'Escape' || e.keyCode === 27) {
                var $modal = $('[id$="pnlModal"]:visible');
                if ($modal.length > 0) {
                    EduApp.closeModal();
                }
            }
        });
    });

})(jQuery);
