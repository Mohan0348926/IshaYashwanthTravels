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

    // Booking Form AJAX Submit
    const bookingForm = document.getElementById("bookingForm");

    if (bookingForm) {
        bookingForm.addEventListener("submit", function (e) {
            e.preventDefault();

            const responseDiv = document.getElementById("bookingResponse");
            const submitBtn = bookingForm.querySelector("button[type=submit]");
            const formData = new FormData(bookingForm);

            submitBtn.disabled = true;
            submitBtn.textContent = "Sending...";

            fetch("/Home/SubmitBooking", {
                method: "POST",
                body: formData
            })
                .then(res => res.json())
                .then(data => {
                    responseDiv.innerHTML =
                        '<div class="alert ' + (data.success ? "alert-success" : "alert-danger") + '">' + data.message + '</div>';

                    if (data.success) {
                        bookingForm.reset();
                    }
                })
                .catch(function () {
                    responseDiv.innerHTML =
                        '<div class="alert alert-danger">Something went wrong. Please try again.</div>';
                })
                .finally(function () {
                    submitBtn.disabled = false;
                    submitBtn.textContent = "Submit";
                });
        });
    }
});