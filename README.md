# us-ac-re
The repetition of user actions

Проект приостановлен, но не закрыт.
Функциональное тестирование UI приложений Windows. 
Построение дерева компонентов, от Desktop до целевого компонента, над которым находится курсор мыши. Определение нажатия клавиш клавиатуры.
Определение компонента под мышью в: 
- практически все приложения с окнами, стабильно определяются компоненты в WinForms, Wpf.
- Web сайты в Chrome, FF, Edge и IE

UsAcRe.Recorder.UI - запись действий пользователя
UsAcRe.Player - воспроизведение ранее записанного скрипта (результат в формате xunit)

в бранче WebApp, начато веб приложение, Blazor. Для удаленного управления запуском скриптов тестиррования:
- UsAcRe.Web.Client
- UsAcRe.Web.Server
