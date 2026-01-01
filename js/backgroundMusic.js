window.backgroundMusic = {
    audio: null,
    isPlaying: false,
    init: function (src) {
        if (this.audio) {
            return;
        }

        this.audio = new Audio(src);
        this.audio.loop = true;
        this.audio.volume = 0;

        const attemptPlay = () => {
            if (this.isPlaying) return;

            this.audio.play().then(() => {
                this.isPlaying = true;
                this.fadeIn();
                this.removeEventListeners();
            }).catch(error => {
                // Autoplay was prevented
                console.log("Autoplay prevented. Waiting for user interaction to start music.");
            });
        };

        this.addEventListeners(attemptPlay);

        // Also try to play immediately
        attemptPlay();
    },
    addEventListeners: function (handler) {
        this.handler = handler;
        window.addEventListener('click', this.handler);
        window.addEventListener('scroll', this.handler);
        window.addEventListener('touchstart', this.handler);
        window.addEventListener('keydown', this.handler);
    },
    removeEventListeners: function () {
        if (this.handler) {
            window.removeEventListener('click', this.handler);
            window.removeEventListener('scroll', this.handler);
            window.removeEventListener('touchstart', this.handler);
            window.removeEventListener('keydown', this.handler);
        }
    },
    fadeIn: function () {
        if (!this.audio) return;

        let volume = 0;
        const targetVolume = 0.5; // Set to a reasonable level, or 1.0 if desired
        const duration = 3000; // 3 seconds fade in
        const interval = 50;
        const step = (targetVolume / (duration / interval));

        const fadeTimer = setInterval(() => {
            volume += step;
            if (volume >= targetVolume) {
                volume = targetVolume;
                clearInterval(fadeTimer);
            }
            this.audio.volume = volume;
        }, interval);
    }
};