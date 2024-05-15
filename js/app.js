document.addEventListener("DOMContentLoaded", function() {
  setTimeout(() => {
    document.body.classList.remove("preload");
  }, 3000);
});

function showSection(sectionId) {
  const sections = document.querySelectorAll('section');
  sections.forEach(section => {
    section.classList.remove('active');
  });
  const activeSection = document.getElementById(sectionId);
  activeSection.classList.add('active');
}
