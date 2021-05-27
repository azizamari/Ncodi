const questions = document.querySelectorAll(".accordion_item")
questions.forEach(question => question.addEventListener("click", () => {
    const currentAnswer = question.querySelector(".answer");
    if (currentAnswer.classList.contains("openAnswer")) {
        currentAnswer.classList.remove("openAnswer");
        question.querySelector(".accordion_link .bx-plus").style.display = "block";
        question.querySelector(".accordion_link .bx-minus").style.display = "none";
    }
    else {
        let open = document.querySelectorAll(".openAnswer");
        if (open.length !== 0) {
            open[0].classList.remove("openAnswer");
            open[0].parentElement.querySelector(".accordion_link .bx-plus").style.display = "block";
            open[0].parentElement.querySelector(".accordion_link .bx-minus").style.display = "none";
        }
        currentAnswer.classList.add("openAnswer");
        question.querySelector(".accordion_link .bx-plus").style.display = "none";
        question.querySelector(".accordion_link .bx-minus").style.display = "block";
    }
}));