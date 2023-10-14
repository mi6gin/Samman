// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function toggleTheme() {
    var currentTheme = sessionStorage.getItem("Theme");
    var newTheme = currentTheme === "dark" ? "light" : "dark";
    sessionStorage.setItem("Theme", newTheme);

    // Применить новую тему к интерфейсу
    document.body.classList.remove(currentTheme + "-theme");
    document.body.classList.add(newTheme + "-theme");

    // Обновить текст кнопки
    var themeToggleButton = document.getElementById("themeButton");
    themeToggleButton.textContent = newTheme === "dark" ? "темная" : "светлая";
}

document.addEventListener("DOMContentLoaded", function () {
    var currentTheme = sessionStorage.getItem("Theme");
    if (currentTheme === "dark" || currentTheme === "light") {
        // Применить стили для текущей темы
        document.body.classList.add(currentTheme + "-theme");
    } else {
        // Если тема не установлена в сессии, по умолчанию используйте темную тему
        document.body.classList.add("light-theme");
        sessionStorage.setItem("Theme", "light"); // Установите тему "light" в сессии по умолчанию
    }

    // Обновить текст кнопки при загрузке страницы
    var themeToggleButton = document.getElementById("themeButton");
    themeToggleButton.textContent = currentTheme === "dark" ? "темная" : "светлая";
});


