window.rsvpForm = (function () {
  // Google reCAPTCHA v2 helpers
  function getRecaptchaSiteKey() {
    try {
      const meta = document.querySelector('meta[name="recaptcha-sitekey"]');
      return meta && meta.content ? meta.content.trim() : '';
    } catch (_) {
      return '';
    }
  }

  function renderRecaptcha() {
    try {
      if (window.grecaptcha && typeof window.grecaptcha.render === 'function') {
        const el = document.querySelector('.g-recaptcha');
        if (el && !el.getAttribute('data-grecaptcha-rendered')) {
          if (!el.getAttribute('data-sitekey')) {
            const key = getRecaptchaSiteKey();
            if (key) el.setAttribute('data-sitekey', key);
          }
          window.grecaptcha.render(el);
          el.setAttribute('data-grecaptcha-rendered', '1');
        }
      }
    } catch (_) { /* no-op */ }
  }

  function getRecaptchaToken() {
    try {
      const el = document.querySelector('textarea[name="g-recaptcha-response"]');
      return el && el.value ? el.value : null;
    } catch (_) {
      return null;
    }
  }

  function resetRecaptcha() {
    try {
      if (window.grecaptcha && typeof window.grecaptcha.reset === 'function') {
        window.grecaptcha.reset();
      }
    } catch (_) { /* no-op */ }
  }

  return {
    getRecaptchaSiteKey,
    renderRecaptcha,
    getRecaptchaToken,
    resetRecaptcha
  };
})();