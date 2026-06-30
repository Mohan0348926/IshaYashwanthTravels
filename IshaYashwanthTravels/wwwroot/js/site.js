document.addEventListener("DOMContentLoaded", function () {



    // Scroll Animation


    const elements = document.querySelectorAll(
        ".stat-card, .feature-card, .service-card, .review-card"
    );



    const observer = new IntersectionObserver(
        entries => {


            entries.forEach(entry => {


                if (entry.isIntersecting) {


                    entry.target.classList.add("animate");
                    entry.target.classList.add("show");


                }


            });



        },
        {
            threshold: 0.2
        }


    );



    elements.forEach(item => {


        item.classList.add("animate");

        observer.observe(item);


    });





    // Navbar background on scroll


    window.addEventListener("scroll", function () {


        let navbar = document.querySelector(".custom-navbar");



        if (window.scrollY > 50) {


            navbar.style.background = "rgba(0,0,0,0.9)";


        }

        else {


            navbar.style.background = "rgba(0,0,0,0.45)";


        }


    });





});