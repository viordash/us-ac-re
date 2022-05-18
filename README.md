# us-ac-re
The repetition of user actions

*Проект приостановлен, но не закрыт.*

Функциональное тестирование приложений в Windows. Построение дерева компонентов, от Desktop до целевого компонента, над которым находится курсор мыши.  

Определение нажатия клавиш клавиатуры.

Определение компонента под мышью в: 
- практически все приложения с окнами, стабильно определяются компоненты в WinForms, Wpf.
- Web сайты в Chrome, FF, Edge и IE

Законченные компоненты проекта:
- UsAcRe.Recorder.UI - запись действий пользователя. Можно добавлять паузы или вставлять ранее сохраненные скрипты для унификации одинаковых действий.
- UsAcRe.Player - воспроизведение ранее записанного скрипта (результат в формате xunit). При воспроизведении скрипта, проверяется/ожидается наличие компонента, если не найден/таймаут, то выполнение прерывается

в бранче WebApp, начато веб приложение, Blazor. Для удаленного управления запуском скриптов тестирования:
- UsAcRe.Web.Client
- UsAcRe.Web.Server

Скрипты основаны на C#, примерный код, *клик по файлам в explorer*:

    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using UsAcRe.Core.Actions;
    using UsAcRe.Core.MouseProcess;
    using UsAcRe.Core.Services;
    using UsAcRe.Core.UIAutomationElement;
    
    namespace UsAcRe.TestsScripts {
    	public class TestsScript {
    		public async Task ExecuteAsync() {
    			await ElementMatchAction.Play(new ElementProgram(0, "explorer"), new List<UiElement>() {
    				new UiElement(0, "", "333 - 3 running windows", "", "", 50000, new System.Windows.Rect(631, 1050, 161, 30)),
    				new UiElement(0, "", "Running applications", "MSTaskListWClass", "", 50021, new System.Windows.Rect(108, 1050, 2228, 30)),
    				new UiElement(0, "", "", "ReBarWindow32", "40965", 50033, new System.Windows.Rect(106, 1050, 2230, 30)),
    				new UiElement(0, "", "", "Shell_TrayWnd", "", 50033, new System.Windows.Rect(0, 1050, 2560, 30)),
    			});
    			await MouseClickAction.Play(MouseButtonType.Left, new System.Drawing.Point(690, 1066), false);
    			await ElementMatchAction.Play(new ElementProgram(1, "explorer"), new List<UiElement>() {
    				new UiElement(0, "TestCome_size_less_and_curr_ver.cfg", "Name", "UIProperty", "System.ItemNameDisplay", 50004, new System.Windows.Rect(579, 666, 268, 22)),
    				new UiElement(0, "", "TestCome_size_less_and_curr_ver.cfg", "UIItem", "2", 50007, new System.Windows.Rect(557, 666, 734, 22)),
    				new UiElement(0, "", "Items View", "UIItemsView", "", 50008, new System.Windows.Rect(543, 593, 793, 396)),
    				new UiElement(0, "", "Shell Folder View", "DUIListView", "listview", 50033, new System.Windows.Rect(543, 593, 793, 396)),
    				new UiElement(0, "", "", "DUIViewWndClassName", "", 50033, new System.Windows.Rect(188, 593, 1148, 419)),
    				new UiElement(0, "", "333", "ShellTabWindowClass", "", 50033, new System.Windows.Rect(188, 593, 1148, 419)),
    				new UiElement(0, "", "333", "CabinetWClass", "", 50032, new System.Windows.Rect(180, 411, 1164, 609)),
    			});
    			await MouseClickAction.Play(MouseButtonType.Left, new System.Drawing.Point(630, 676), false);
    			await ElementMatchAction.Play(new ElementProgram(1, "explorer"), new List<UiElement>() {
    				new UiElement(0, "TestCome_size_larger_and_curr_ver.cfg", "Name", "UIProperty", "System.ItemNameDisplay", 50004, new System.Windows.Rect(579, 624, 268, 22)),
    				new UiElement(0, "", "TestCome_size_larger_and_curr_ver.cfg", "UIItem", "0", 50007, new System.Windows.Rect(557, 624, 734, 22)),
    				new UiElement(0, "", "Items View", "UIItemsView", "", 50008, new System.Windows.Rect(543, 593, 793, 396)),
    				new UiElement(0, "", "Shell Folder View", "DUIListView", "listview", 50033, new System.Windows.Rect(543, 593, 793, 396)),
    				new UiElement(0, "", "", "DUIViewWndClassName", "", 50033, new System.Windows.Rect(188, 593, 1148, 419)),
    				new UiElement(0, "", "333", "ShellTabWindowClass", "", 50033, new System.Windows.Rect(188, 593, 1148, 419)),
    				new UiElement(0, "", "333", "CabinetWClass", "", 50032, new System.Windows.Rect(180, 411, 1164, 609)),
    			});
    			await MouseClickAction.Play(MouseButtonType.Left, new System.Drawing.Point(652, 636), false);
    
    		}
    	}
    }

