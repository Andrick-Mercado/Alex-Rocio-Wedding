window.dressSwipe = (function () {
  const map = new WeakMap();

  function init(el, dotnet) {
    if (!el || !dotnet) return;
    let startX = 0, startY = 0, active = false;

    const getPoint = (e) => (e.touches && e.touches[0]) || (e.changedTouches && e.changedTouches[0]) || e;

    const onStart = (e) => {
      const p = getPoint(e);
      startX = p.clientX;
      startY = p.clientY;
      active = true;
    };

    const onEnd = (e) => {
      if (!active) return;
      active = false;
      const p = getPoint(e);
      const dx = p.clientX - startX;
      const dy = p.clientY - startY;

      // Horizontal swipe threshold
      if (Math.abs(dx) > 40 && Math.abs(dx) > Math.abs(dy)) {
        const dir = dx < 0 ? 'left' : 'right';
        try { dotnet.invokeMethodAsync('OnDressSwipe', dir); } catch { }
      }
    };

    el.addEventListener('touchstart', onStart, { passive: true });
    el.addEventListener('touchend', onEnd, { passive: true });
    // Basic mouse support for desktop
    el.addEventListener('mousedown', onStart);
    el.addEventListener('mouseup', onEnd);

    map.set(el, { onStart, onEnd });
  }

  function dispose(el) {
    const h = map.get(el);
    if (!h || !el) return;
    el.removeEventListener('touchstart', h.onStart);
    el.removeEventListener('touchend', h.onEnd);
    el.removeEventListener('mousedown', h.onStart);
    el.removeEventListener('mouseup', h.onEnd);
    map.delete(el);
  }

  return { init, dispose };
})();