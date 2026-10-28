"use strict";

const $ = id => document.getElementById(id);

const connection = new signalR.HubConnectionBuilder()
    .withUrl("hubs/poker")
    .withAutomaticReconnect()
    .build();

let session = null; // { room, name, observer }
let myVote = null;

function storage(fn) {
    try { return fn(window.localStorage); } catch { return null; }
}

function setStatus(text) {
    $("status").textContent = text || "";
}

function randomRoomId() {
    const words = ["otter", "falcon", "maple", "comet", "harbor", "cedar", "pixel", "tiger"];
    return words[Math.floor(Math.random() * words.length)] + "-" + Math.floor(1000 + Math.random() * 9000);
}

function inviteLink(room) {
    const url = new URL(window.location.href);
    url.search = "";
    url.hash = "";
    url.searchParams.set("room", room);
    return url.toString();
}

async function invoke(method, ...args) {
    try {
        setStatus("");
        await connection.invoke(method, ...args);
    } catch (err) {
        setStatus((err.message || String(err)).replace(/^.*HubException: /, ""));
    }
}

async function join(room, name, observer) {
    session = { room, name, observer };
    storage(s => s.setItem("poker.name", name));
    history.replaceState(null, "", inviteLink(room));
    await invoke("JoinRoom", room, name, observer);
}

function render(state) {
    session.room = state.roomId;
    $("join-view").hidden = true;
    $("room-view").hidden = false;
    $("room-info").hidden = false;
    $("room-name").textContent = state.roomId;

    const storyInput = $("story-input");
    if (document.activeElement !== storyInput) storyInput.value = state.story;

    const me = state.participants.find(p => p.connectionId === connection.connectionId);
    if (!me || !me.hasVoted) myVote = null;
    else if (state.revealed) myVote = me.vote;

    const list = $("participants");
    list.replaceChildren(...state.participants.map(p => {
        const li = document.createElement("li");
        li.className = "participant" + (p === me ? " me" : "");

        const slot = document.createElement("div");
        if (p.isObserver) {
            slot.className = "slot observer";
            slot.textContent = "👀";
        } else if (state.revealed && p.hasVoted) {
            slot.className = "slot shown";
            slot.textContent = p.vote;
        } else {
            slot.className = "slot" + (p.hasVoted ? " voted" : "");
            slot.title = p.hasVoted ? "Voted" : "Thinking…";
        }

        const name = document.createElement("div");
        name.className = "name";
        name.textContent = p.name + (p === me ? " (you)" : "");
        li.append(slot, name);
        return li;
    }));

    const voters = state.participants.filter(p => !p.isObserver);
    const voted = voters.filter(p => p.hasVoted).length;
    $("reveal-btn").disabled = state.revealed || voted === 0;
    $("reveal-btn").textContent = state.revealed ? "Votes revealed" : `Reveal votes (${voted}/${voters.length})`;

    renderSummary(state);
    renderDeck(state, me);
}

function renderSummary(state) {
    const summary = $("summary");
    summary.hidden = !state.revealed;
    if (!state.revealed || !state.summary) return;

    const s = state.summary;
    const parts = [];
    if (s.voteCount === 0) {
        parts.push("No votes were cast.");
    } else {
        if (s.consensus) parts.push(`<span class="consensus">Consensus on ${escapeHtml(s.distribution[0].card)}!</span>`);
        if (s.average !== null) parts.push(`Average: <strong>${s.average}</strong>`);
    }
    const bars = s.distribution
        .map(d => `<span class="bar">${escapeHtml(d.card)} × ${d.count}</span>`)
        .join("");
    summary.innerHTML = `<div>${parts.join(" · ")}</div><div class="bars">${bars}</div>`;
}

function renderDeck(state, me) {
    const canVote = me && !me.isObserver && !state.revealed;
    $("deck-wrap").hidden = !me || me.isObserver;
    $("deck-title").textContent = state.revealed ? "Votes are revealed. Start a new round to vote again." : "Pick your card";

    $("deck").replaceChildren(...state.deck.map(card => {
        const btn = document.createElement("button");
        btn.type = "button";
        btn.textContent = card;
        btn.disabled = !canVote;
        if (card === myVote) btn.classList.add("selected");
        btn.addEventListener("click", () => {
            myVote = myVote === card ? null : card;
            invoke("Vote", card);
        });
        return btn;
    }));
}

function escapeHtml(text) {
    const div = document.createElement("div");
    div.textContent = text;
    return div.innerHTML;
}

connection.on("roomState", render);

connection.onreconnecting(() => setStatus("Connection lost, reconnecting…"));
connection.onreconnected(async () => {
    setStatus("");
    if (session) await invoke("JoinRoom", session.room, session.name, session.observer);
});
connection.onclose(() => setStatus("Disconnected. Refresh the page to rejoin."));

$("join-form").addEventListener("submit", e => {
    e.preventDefault();
    join($("room-input").value.trim(), $("name-input").value.trim(), $("observer-input").checked);
});

$("story-form").addEventListener("submit", e => {
    e.preventDefault();
    invoke("SetStory", $("story-input").value);
    $("story-input").blur();
});

$("reveal-btn").addEventListener("click", () => invoke("Reveal"));
$("reset-btn").addEventListener("click", () => invoke("Reset"));

$("copy-link").addEventListener("click", async () => {
    try {
        await navigator.clipboard.writeText(inviteLink(session.room));
        setStatus("Invite link copied.");
    } catch {
        setStatus("Copy this link: " + inviteLink(session.room));
    }
});

(async function start() {
    const params = new URLSearchParams(window.location.search);
    $("room-input").value = params.get("room") || randomRoomId();
    $("name-input").value = storage(s => s.getItem("poker.name")) || "";
    if (params.get("room")) $("name-input").focus();

    try {
        await connection.start();
    } catch (err) {
        setStatus("Could not connect to the server: " + err);
    }
})();
