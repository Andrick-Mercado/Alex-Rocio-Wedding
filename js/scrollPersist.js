// Seamless scroll position persistence across refreshes
(function(){
    try {
        if ('scrollRestoration' in history) {
            history.scrollRestoration = 'manual';
        }
        const storage = window.sessionStorage;
        const key = function(){ return 'scroll:' + location.pathname + location.search; };

        function save(){
            try { storage.setItem(key(), String(Math.round(window.scrollY || window.pageYOffset || 0))); } catch(e) {}
        }

        function restore(){
            const raw = storage.getItem(key());
            if(!raw) return;
            const target = parseInt(raw, 10);
            if(isNaN(target)) return;

            let attempts = 0;
            const maxAttempts = 120; // ~2s at 60fps
            const snap = function(){
                // If content isn't tall enough yet, keep trying
                const docHeight = Math.max(document.body.scrollHeight, document.documentElement.scrollHeight);
                const near = Math.abs((window.scrollY||0) - target) < 1;
                if (!near) {
                    window.scrollTo(0, target);
                }
                if (docHeight < target + 1 && attempts < maxAttempts) {
                    attempts++;
                    requestAnimationFrame(snap);
                }
            };
            // Try ASAP and then keep nudging until content settles
            requestAnimationFrame(snap);
        }

        // Save early and often
        window.addEventListener('beforeunload', save, { passive: true });
        document.addEventListener('visibilitychange', function(){ if(document.hidden) save(); }, { passive: true });

        // Restore as early as possible
        window.addEventListener('DOMContentLoaded', restore, { once: true, passive: true });
        window.addEventListener('pageshow', function(e){ if (e.persisted) restore(); }, { passive: true });
    } catch(err) {
        // no-op; never block app
    }
})();