
#!/usr/bin/env bash
set -euo pipefail

LOG_DIR="$HOME/.config/VintagestoryData/Logs"
BACKUP_DIR="$HOME/git/games/livestockmarker/.debug-logs"

MODE="${1:-clear}"

if [[ ! -d "$LOG_DIR" ]]; then
    echo "Nie znaleziono katalogu: $LOG_DIR"
    exit 1
fi

case "$MODE" in
    backup)
        mkdir -p "$BACKUP_DIR"

        ARCHIVE="$BACKUP_DIR/logs-$(date +%Y%m%d-%H%M%S).tar.gz"

        tar -czf "$ARCHIVE" -C "$LOG_DIR" \
            --exclude='*.lck' .

        echo "Kopia logów: $ARCHIVE"
        ;;
    clear)
        find "$LOG_DIR" -maxdepth 1 \
            -type f -name '*.log' -delete

        echo "Usunięto logi *.log z $LOG_DIR"
        ;;
    *)
        echo "Użycie: $0 [clear|backup]"
        exit 1
        ;;
esac
