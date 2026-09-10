document.addEventListener("DOMContentLoaded", function () {

    // ================= SCROLL ANIMATION =================

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


    // ================= NAVBAR BACKGROUND =================

    window.addEventListener("scroll", function () {

        let navbar = document.querySelector(".custom-navbar");

        if (!navbar) return;

        if (window.scrollY > 50) {

            navbar.style.background = "rgba(0,0,0,0.9)";

        }
        else {

            navbar.style.background = "rgba(0,0,0,0.45)";

        }

    });


    // ================= BOOKING FORM =================

    const bookingForm = document.getElementById("bookingForm");

    if (bookingForm) {

        // Prevent selecting previous dates
        const travelDate = document.getElementById("travelDate");

        if (travelDate) {

            const today = new Date();

            const year = today.getFullYear();
            const month = String(today.getMonth() + 1).padStart(2, "0");
            const day = String(today.getDate()).padStart(2, "0");

            travelDate.min = `${year}-${month}-${day}`;
        }


        bookingForm.addEventListener("submit", function (e) {

            e.preventDefault();

            const responseDiv =
                document.getElementById("bookingResponse");

            const submitBtn =
                document.getElementById("bookingSubmitBtn");

            const btnText =
                document.getElementById("bookingBtnText");

            const spinner =
                document.getElementById("bookingSpinner");


            const formData = new FormData(bookingForm);


            // Disable button
            submitBtn.disabled = true;

            if (btnText) {

                btnText.innerHTML =
                    '<i class="fa-solid fa-spinner fa-spin me-2"></i>' +
                    'Sending Booking...';

            }

            if (spinner) {

                spinner.classList.remove("d-none");

            }


            fetch("/Home/SubmitBooking", {

                method: "POST",
                body: formData

            })

                .then(res => {

                    if (!res.ok) {

                        throw new Error("Server error");

                    }

                    return res.json();

                })

                .then(data => {

                    if (data.success) {

                        responseDiv.innerHTML =
                            '<div class="alert alert-success">' +
                            '<i class="fa-solid fa-circle-check me-2"></i>' +
                            data.message +
                            '</div>';


                        // Clear form
                        bookingForm.reset();


                        // Hide modal after 3 seconds
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

                            responseDiv.innerHTML = "";

                        }, 3000);

                    }
                    else {

                        responseDiv.innerHTML =
                            '<div class="alert alert-danger">' +
                            '<i class="fa-solid fa-triangle-exclamation me-2"></i>' +
                            data.message +
                            '</div>';

                    }

                })

                .catch(function (error) {

                    console.error(error);

                    responseDiv.innerHTML =
                        '<div class="alert alert-danger">' +
                        '<i class="fa-solid fa-triangle-exclamation me-2"></i>' +
                        'Something went wrong. Please try again.' +
                        '</div>';

                })

                .finally(function () {

                    submitBtn.disabled = false;

                    if (btnText) {

                        btnText.innerHTML =
                            '<i class="fa-solid fa-paper-plane me-2"></i>' +
                            'Submit Booking';

                    }

                    if (spinner) {

                        spinner.classList.add("d-none");

                    }

                });

        });

    }

});