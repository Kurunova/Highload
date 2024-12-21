box.cfg{
    listen = 3301
}

-- Инициализация пространства, если это не сделано
if not box.space.dialog_messages then
    box.schema.space.create('dialog_messages', {
        format = {
            {name = 'id', type = 'unsigned'},
            {name = 'dialog_id', type = 'string'},
            {name = 'from_user_id', type = 'unsigned'},
            {name = 'to_user_id', type = 'unsigned'},
            {name = 'text', type = 'string'},
            {name = 'sent_at', type = 'string'},
        }
    })
    -- Создаем составной первичный ключ (dialog_id, id)
    box.space.dialog_messages:create_index('primary', {
        parts = {
            {field = 'dialog_id', type = 'string'},
            {field = 'id', type = 'unsigned'}
        },
        if_not_exists = true
    })
    -- Дополнительные индексы
    box.space.dialog_messages:create_index('dialog_id_index', {
        parts = {'dialog_id'},
        unique = false,
        if_not_exists = true
    })
end

print("Tarantool instance is configured and running!")