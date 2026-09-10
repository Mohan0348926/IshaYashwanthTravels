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

        const navbar = document.querySelector(".custom-navbar");

        if (!navbar) return;

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

        bookingForm.addEventListener("submit", async function (e) {

            e.preventDefault();

            const responseDiv = document.getElementById("bookingResponse");
            const submitBtn = bookingForm.querySelector("button[type=submit]");

            // FormData automatically includes:
            // Name
            // Email
            // Phone
            // TravelDate
            // From
            // To
            // Passengers
            // TripType
            // VehicleType
            // Message
            const formData = new FormData(bookingForm);

            submitBtn.disabled = true;
            submitBtn.innerHTML =
                '<span class="spinner-border spinner-border-sm me-2"></span>Sending...';

            responseDiv.innerHTML = "";

            try {

                const response = await fetch("/Home/SubmitBooking", {
                    method: "POST",
                    body: formData
                });

                if (!response.ok) {
                    throw new Error("Server error: " + response.status);
                }

                const data = await response.json();

                if (data.success) {

                    responseDiv.innerHTML = `
                        <div class="alert alert-success">
                            <i class="fas fa-check-circle me-2"></i>
                            ${data.message}
                        </div>
                    `;

                    // Clear form
                    bookingForm.reset();

                    // Optional: close modal after successful booking
                    setTimeout(function () {

                        const modalElement =
                            document.getElementById("bookingModal");

                        if (modalElement) {

                            const modal =
                                bootstrap.Modal.getInstance(modalElement);

                            if (modal) {
                                modal.hide();
                            }
                        }

                    }, 2500);

                }
                else {

                    responseDiv.innerHTML = `
                        <div class="alert alert-danger">
                            <i class="fas fa-exclamation-circle me-2"></i>
                            ${data.message}
                        </div>
                    `;
                }

            }
            catch (error) {

                console.error("Booking submission error:", error);

                responseDiv.innerHTML = `
                    <div class="alert alert-danger">
                        <i class="fas fa-exclamation-circle me-2"></i>
                        Something went wrong. Please try again.
                    </div>
                `;

            }
            finally {

                submitBtn.disabled = false;
                submitBtn.innerHTML = "Submit";

            }

        });
    }

});
