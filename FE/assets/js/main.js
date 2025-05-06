// Set the date we're counting down to (30 days from now)
const countDownDate = new Date("2025-09-06T07:00:00");

// Update the countdown every 1 second
const countdown = setInterval(function () {
    // Get today's date and time
    const now = new Date().getTime();

    // Find the distance between now and the countdown date
    const distance = countDownDate - now;

    // Time calculations for days, hours, minutes and seconds
    const days = Math.floor(distance / (1000 * 60 * 60 * 24));
    const hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    const minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
    const seconds = Math.floor((distance % (1000 * 60)) / 1000);

    // Display the result
    document.getElementById("days").textContent = String(days).padStart(2, '0');
    document.getElementById("hours").textContent = String(hours).padStart(2, '0');
    document.getElementById("minutes").textContent = String(minutes).padStart(2, '0');
    document.getElementById("seconds").textContent = String(seconds).padStart(2, '0');

    // If the countdown is finished, write some text
    if (distance < 0) {
        clearInterval(countdown);
        document.getElementById("countdown").innerHTML = "Sự kiện đã kết thúc!";
    }
}, 1000);

// Smooth scroll for navigation links
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();
        document.querySelector(this.getAttribute('href')).scrollIntoView({
            behavior: 'smooth'
        });
    });
});

// Add animation on scroll
const animateOnScroll = () => {
    const elements = document.querySelectorAll('.guide-item, .section-title');

    elements.forEach(element => {
        const elementTop = element.getBoundingClientRect().top;
        const elementBottom = element.getBoundingClientRect().bottom;

        if (elementTop < window.innerHeight && elementBottom > 0) {
            element.style.opacity = '1';
            element.style.transform = 'translateY(0)';
        }
    });
};

// Initial styles for animation
document.querySelectorAll('.guide-item, .section-title').forEach(element => {
    element.style.opacity = '0';
    element.style.transform = 'translateY(20px)';
    element.style.transition = 'all 0.6s ease-out';
});

// Listen for scroll events
window.addEventListener('scroll', animateOnScroll);
// Initial check for elements in view
window.addEventListener('load', animateOnScroll);

// Video Player Functionality
function playVideo() {
    const video = document.getElementById('intro-video');
    const videoWrapper = video.parentElement;

    video.play();
    videoWrapper.classList.add('video-playing');

    // Remove overlay when video starts playing
    video.addEventListener('play', function () {
        videoWrapper.classList.add('video-playing');
    });

    // Show overlay when video is paused
    video.addEventListener('pause', function () {
        videoWrapper.classList.remove('video-playing');
    });
}

// Form validation and submission
document.addEventListener('DOMContentLoaded', function () {
    const registrationForm = document.getElementById('registrationForm');
    if (registrationForm) {
        // Form validation
        registrationForm.addEventListener('submit', function (event) {
            event.preventDefault();
            event.stopPropagation();

            if (registrationForm.checkValidity()) {
                // Lưu thông tin người dùng vào localStorage
                const userInfo = {
                    fullName: document.getElementById('fullName').value,
                    birthDate: document.getElementById('birthDate').value,
                    unit: document.getElementById('unit').value,
                    phone: document.getElementById('phone').value
                };
                localStorage.setItem('userInfo', JSON.stringify(userInfo));

                // Chuyển hướng đến trang thi
                window.location.href = 'exam.html';
            }

            registrationForm.classList.add('was-validated');
        });

        // Phone number validation
        const phoneInput = document.getElementById('phone');
        if (phoneInput) {
            phoneInput.addEventListener('input', function (e) {
                let phone = e.target.value.replace(/\D/g, '');
                if (phone.length > 10) {
                    phone = phone.slice(0, 10);
                }
                e.target.value = phone;
            });
        }
    }
}); 