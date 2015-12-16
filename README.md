#Light and native javascript bb-editor

#Usage
1.Download sources from github and unpack them

2.Insert on to your html page links to css and js files

      <link rel="stylesheet" href="/nativejsbb.min.css" />
      <scrtipt src="/nativejsbb.min.js"></script>
  
3.On you page you should to create a textarea

4.Init plugin

      $(function() { new NativeJSBBEditor({ target: 'bbeditor' }) });

  or by using native js

      document.addEventListener('DOMContentLoaded', function () { new NativeJSBBEditor({ target: 'bbeditor' }) });
  
5....

6.PROFIT!


#Examples

      document.addEventListener('DOMContentLoaded', function () {
        
        var availableTags = [
        
            { tag: 'b' },                               //flat tags, like bold, italic etc...
            { tag: 'u', space: false },
            { tag: 's', space: false, caption: 'S' },   //flat tag with caption on a button
            { space: true },                            //just space
            {
                tag: 'color',               //tag
                modal: {                    //modal window will be shown
                  Title: 'Font color',      //modal window title
                  Width: 300,               //modal window width (numeric, default 200)
                  ApplyButton: 'apply',     //apply button caption (by default caption is OK)
                  CloseButton: 'close',     //close (modal closer) button caption (by default caption is cancel)
                  Fields: [{                //modal window fields description. dropdown select with options
                    Label: 'Color',         //field label
                    Type: 'dropdown',       //field type (input or checkbox, input by default)
                    TagOption: 'my_own_prop',     //tag option is my_own_prop, so it seems like that: [color my_own_prop='green']colored text[/color]
                    Values: [{
                        Value: 'red',
                        Text: 'red color'
                      }, {
                        Value: 'green',
                        Text: 'green color'
                      }, {
                        Value: 'blue',
                        Text: 'blue color'
                      }]
                    }]
                }
            },
            {
              tag: 'link',
              modal: {
                Title: 'Insert link',
                Width: 300,
                Fields: [{                  //one input field with default value
                  Label: 'Ссылка',
                  Type: 'text',
                  DefaultValue: 'http://',
                  TagOption: 'url'
                }]
              }
            }];
            
            new NativeJSBBEditor({ target: 'bbeditor', tags: availableTags }); //init plugin and apply it to #bbeditor
      });
  
#Transform BB to html-tags on backend
  
  To transform posted data to html - you should use c# parser, it's specially designed for the nativeJS bb-editor.
  
  Project including c# parser source (BBParser.cs)
