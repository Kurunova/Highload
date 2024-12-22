box.cfg{
    listen = 3301
}

-- Инициализация последовательности
if not box.sequence.dialog_messages_seq then
    box.schema.sequence.create('dialog_messages_seq', {min = 1, start = 1, if_not_exists = true})
end

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

function add_dialog_message(dialog_id, from_user_id, to_user_id, text, sent_at)
    local id = box.sequence.dialog_messages_seq:next()
    return box.space.dialog_messages:insert{id, dialog_id, from_user_id, to_user_id, text, sent_at}
end

function get_dialog_messages(dialog_id)
    return box.space.dialog_messages:select{dialog_id}
end

box.schema.func.create('add_dialog_message', {if_not_exists = true})
box.schema.func.create('get_dialog_messages', {if_not_exists = true})

print("Tarantool instance is configured and running!")