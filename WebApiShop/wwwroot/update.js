async function update() {
    const username = document.querySelector("#userName");
    const password = document.querySelector("#pass")
    const firstName = document.querySelector("#firstName")
    const lastName = document.querySelector("#lastName")
    const q = JSON.parse(sessionStorage.getItem("User"))
    const updateData = {
        Email: username.value,
        Password: password.value,
        FirstName: firstName.value,
        LastName: lastName.value,
    };
    try {
        const response = await fetch(`/api/users/${q.id}`,
            {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(updateData)
            });
        if (!response.ok) {
            throw new Error(`HTTP error! status"${response.status}"`)
        }
        const dataPut = await response.json();
        sessionStorage.setItem("User", JSON.stringify(dataPut))
        console.log('Put data:', dataPut);
    }
    catch (e) {
        console.log(e)
    }
    alert("הפרטים עודכנו בהצלחה!")
}
