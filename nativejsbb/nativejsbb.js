/*
* NativeJSBBEditor - Native Javascript BB-edtitor
*   (http://heap.tech)
*
* Copyright (c) 2015 zizzlezee / http://heap.tech/author/zizzlezee
* Dual licensed under the MIT and GPL licenses:
*   http://www.opensource.org/licenses/mit-license.php
*   http://www.gnu.org/licenses/gpl.html
* 
* $Version 1.0.151215
*/
        function NativeJSBBEditor(options) {

            var _target = {};


            var _data = {

                wrap: {},
                
                watermark: {},

                tagElements_wrap: {},

                tagElements: [],

                textElement_wrap: {},

                textElement: {}

            };
            

            if (this instanceof NativeJSBBEditor) { }
            else {
                return new NativeJSBBEditor(options);
            }



            function modal(props) {


                var render = function () {

                    var screenSize = windowSize();

                    var mConfig = props.modal;

                    var overlay = document.createElement("div"),
                         wrapper = document.createElement("div"),
                         wnd = document.createElement("div"),
                         head = document.createElement("div"),
                         body = document.createElement("div"),
                         buttons_wrap = document.createElement('div'),
                         button_ok = document.createElement('input'),
                         button_close = document.createElement('div');


                    body.className = 'body';


                    head.className = 'head';
                    head.innerHTML = mConfig.Title || "Title";


                    wnd.className = 'wnd';


                    wrapper.className = 'bbeditor__modal__wrapper';
                    wrapper.style.width = (mConfig.Width ? mConfig.Width : 200) + 'px';
                    wrapper.style.left = ((screenSize.Width / 2) - ((mConfig.Width ? mConfig.Width : 200) / 2)) + 'px';


                    overlay.className = 'bbeditor__overlay';                   


                    buttons_wrap.className = 'btn';


                    button_ok.type = 'button';
                    button_ok.value = mConfig.ApplyButton ? mConfig.ApplyButton : 'OK';


                    button_close.type = 'button';
                    button_close.innerHTML = mConfig.CloseButton ? mConfig.CloseButton : 'cancel';
                    button_close.className = 'bbeditor__modal__close';
                    button_close.addEventListener('click', closeHandler);


                    buttons_wrap.appendChild(button_ok);
                    buttons_wrap.appendChild(button_close);


                    wnd.appendChild(head);
                    wnd.appendChild(body);
                    wnd.appendChild(buttons_wrap);


                    wrapper.appendChild(wnd);


                    this.buttonOk = button_ok;

                    this.buttonClose = button_close;

                    this.body = body;

                    this.wrapper = wrapper;

                    this.overlay = overlay;

                    return this;

                }


                var windowSize = function () {

                    return {

                        Width: window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth || window.screen.width || 0,

                        Height: window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight || window.screen.height || 0

                    };

                }


                var closeHandler = function () {

                    var ov = document.getElementsByClassName("bbeditor__overlay");

                    var wnd = document.getElementsByClassName("bbeditor__modal__wrapper");

                    if (ov.length) {

                        for (var i = 0; i < ov.length; i++)
                            ov[i].parentNode.removeChild(ov[i]);

                    }

                    if (wnd.length) {

                        for (var i = 0; i < wnd.length; i++)
                            wnd[i].parentNode.removeChild(wnd[i]);

                    }

                }


                var okHandler = function () {

                    var sender = this;

                    var fieldsValues = [];

                    var inputs = body.getElementsByTagName('input'),
                        selects = body.getElementsByTagName('select');

                    if (inputs.length) {
                        for (var i = 0; i < inputs.length; i++) {
                            fieldsValues.push(inputs[i].getAttribute('tagOption') + '="' + inputs[i].value + '"');
                        }
                    }

                    if (selects.length) {
                        for (var i = 0; i < selects.length; i++)
                            fieldsValues.push(selects[i].getAttribute('tagOption') + '="' + selects[i].options[selects[i].selectedIndex].value + '"');
                    }


                    insertBB(props.tag, fieldsValues);

                }


                var wnd = render();

                /*rendering modal fields (input or select) */
                if (props &&
                    props.modal &&
                    props.modal.Fields &&
                    Object.prototype.toString.call(props.modal.Fields) == "[object Array]") {

                    var label,
                        field;

                    for (var i in props.modal.Fields) {


                        label = document.createElement('label');
                        label.innerHTML = props.modal.Fields[i].Label;


                        if (props.modal.Fields[i].Type == "text" ||
                             props.modal.Fields[i].Type) {


                            field = document.createElement('input');
                            field.type = 'text';
                            field.name = 'f' + i;
                            field.value = (props.modal.Fields[i].DefaultValue ? props.modal.Fields[i].DefaultValue : "");


                        }


                        if (props.modal.Fields[i].Type == "dropdown") {

                            field = document.createElement('select');
                            field.name = 'd' + i;

                            var optionsHtml = [];

                            if (props.modal.Fields[i].Values) {

                                for (var val in props.modal.Fields[i].Values)
                                    optionsHtml.push("<option value='" + props.modal.Fields[i].Values[val].Value + "'>" + props.modal.Fields[i].Values[val].Text + "</option>");

                            }

                            field.innerHTML = optionsHtml.join('');

                        }


                        field.setAttribute('tagOption', props.modal.Fields[i].TagOption);


                        wnd.body.appendChild(label);
                        wnd.body.appendChild(field);

                    }

                }


                /* attach events and create DOM modal and overlay */
                wnd.buttonClose.addEventListener('click', function () { closeHandler() });

                wnd.buttonOk.addEventListener('click', function () { okHandler(); closeHandler(); });

                document.body.appendChild(wnd.overlay);

                document.body.appendChild(wnd.wrapper);

                wnd.wrapper.style.top = (windowSize().Height / 2) - (wrapper.clientWidth / 2) + 'px';

            }


            function insertBB(tag, tagOptions, justInsertTag) {

                var tagStart = tagOptions && tagOptions.length ? `[${tag} ${tagOptions.join('')}]` : `[${tag}]`,
                    tagEnd = `[/${tag}]`;

                var textarea_val = _data.textElement.value;

                var selectionStarPos = _data.textElement.selectionStart;

                var selectionEndPos = _data.textElement.selectionEnd;

                var selectedText = textarea_val.substring(selectionStarPos, selectionEndPos);

                var cursorPosition = 0,
                    compiledText = "";

                if (selectedText &&
                    !justInsertTag) {

                    cursorPosition = selectionEndPos + tagStart.length + tagEnd.length;

                    compiledText = `${textarea_val.substring(0, selectionStarPos)}${tagStart}${selectedText}${tagEnd}${textarea_val.substring(selectionEndPos)}`;

                }
                else {

                    cursorPosition = selectionStarPos + tagStart.length;

                    compiledText = `${textarea_val.substring(0, selectionStarPos)}${tagStart}${tagEnd}${textarea_val.substring(selectionStarPos)}`;

                }

                _data.textElement.value = compiledText;

                _data.textElement.focus();

                _data.textElement.setSelectionRange(cursorPosition, cursorPosition);
            }


            //merge options
            function _extendOptions() {

                var baseOptions = {

                    target: null,
                    tags: [
                        { tag: 'b' },
                        { tag: 's' },
                        { tag: 'i' },
                        { tag: 'u' },
                        { space: true },

                        { space: true },
                        { tag: 'quote' },
                    ],
                    watermark: true

                };

                //merge options
                for (var bO in baseOptions) {

                    if (!options[bO]) {

                        options[bO] = baseOptions[bO];

                    }

                }
                

                if (!options.tags ||
                    !options.tags.length) {

                    options.tags = baseOptions.tags;

                }
            }


            //format bbObject
            function _formHTMLInput() {

                if (!options.target) {

                    console.error('target is not declared');

                }
                else {

                    _target = document.getElementById(options.target);

                    if (_target) {

                        var tag = {};
                        
                        for (var t in options.tags) {

                            tag = document.createElement('div');

                            if (!options.tags[t].space) {

                                if (options.tags[t].caption) {
                                    tag.innerHTML = options.tags[t].tag;
                                }

                                tag.className = 'tag';

                                tag.setAttribute('tag', options.tags[t].tag);

                                //store to tag prototype tag.options
                                tag.BBEditor = options.tags[t];

                            }
                            else tag.className = 'tag space';
                            
                            _data.tagElements.push(tag);

                        }

                        _data.wrap = document.createElement('div');                        
                        _data.wrap.className = 'bbeditor__wrap';


                        _data.watermark = document.createElement('div');
                        _data.watermark.className = 'watermark';
                        _data.watermark.innerHTML = 'NativeJS BB-editor, <a href="http://heap.tech">heap.tech</a> for more info';


                        _data.tagElements_wrap = document.createElement('div');
                        _data.tagElements_wrap.className = 'tags__wrap';


                        _data.textElement_wrap = document.createElement('div');
                        _data.textElement_wrap.className = 'text__wrap';


                        _data.textElement = document.createElement('textarea');                       
                        _data.textElement.id = _target.id;

                    }

                }
            }


            //replace textarea to well-formed BBEditor wrapper
            function _replaceTarget() {
                
                if (_data.wrap &&
                    _data.tagElements_wrap &&
                    _data.tagElements &&
                    _data.textElement_wrap &&
                    _data.textElement) {

                    var container = _target.parentNode;

                    container.removeChild(_target);
                    
                    for (var i in _data.tagElements) {

                        _data.tagElements_wrap.appendChild(_data.tagElements[i]);

                    }                    

                    //append clear div to tags_wrapper
                    var clear = document.createElement('div');
                    clear.style.clear = 'both';
                    _data.tagElements_wrap.appendChild(clear);

                    if (options.watermark) {
                        _data.wrap.appendChild(_data.watermark);
                    }
                    
                    //append textarea to its wrapper
                    _data.textElement_wrap.appendChild(_data.textElement);

                    //append tags to main wrap
                    _data.wrap.appendChild(_data.tagElements_wrap);
                    
                    //append texarea_wrapper to main wrap
                    _data.wrap.appendChild(_data.textElement_wrap);
                    
                    container.appendChild(_data.wrap);
                }

            }
            

            //attach events to created element
            function _attachEvents() {

                for (var i in _data.tagElements) {
                    

                    _data.tagElements[i].addEventListener('click', function () {

                        var sender = this;

                        if (sender.BBEditor) {

                            if (sender.BBEditor.modal)
                                modal(sender.BBEditor);
                            else
                                insertBB(sender.getAttribute('tag'));

                        }
                    });

                }

            }


            _extendOptions();


            _formHTMLInput();


            _replaceTarget();


            _attachEvents();


            return _data.wrap;

        }
