window.scrollFade = (function(){
  let observer;
  function init(){
    const sections = document.querySelectorAll('.fade-section');
    if(!('IntersectionObserver' in window)){
      sections.forEach(s => s.classList.add('show'));
      return;
    }
    observer = new IntersectionObserver((entries)=>{
      entries.forEach(e => {
        if(e.isIntersecting){ e.target.classList.add('show'); }
      });
    },{ threshold: 0.25 });

    sections.forEach(s => observer.observe(s));
  }
  return { init };
})();