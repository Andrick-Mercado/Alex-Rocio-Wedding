window.weddingSnow = (function(){
  let canvas, ctx, width, height;
  let flakes = [];
  const MAX = 160;

  function onResize(){
    width = canvas.width = window.innerWidth;
    height = canvas.height = window.innerHeight;
  }

  function createFlake(){
    return {
      x: Math.random() * width,
      y: Math.random() * -height,
      r: Math.random() * 2.2 + 0.8,
      s: Math.random() * 0.8 + 0.2,
      a: Math.random() * Math.PI * 2
    };
  }

  function update(){
    ctx.clearRect(0,0,width,height);
    flakes.forEach(f => {
      f.y += f.s + f.r * 0.05;
      f.x += Math.sin(f.a += 0.01) * 0.5;
      if (f.y > height + 5) {
        f.y = -10; f.x = Math.random()*width;
      }
      ctx.beginPath();
      ctx.arc(f.x, f.y, f.r, 0, Math.PI*2);
      ctx.fillStyle = 'rgba(255,255,255,0.9)';
      ctx.fill();
    });
    requestAnimationFrame(update);
  }

  function init(){
    canvas = document.getElementById('snow-canvas');
    if(!canvas) return;
    ctx = canvas.getContext('2d');
    onResize();
    flakes = Array.from({length: MAX}, createFlake);
    window.addEventListener('resize', onResize);
    update();
  }

  return { init };
})();